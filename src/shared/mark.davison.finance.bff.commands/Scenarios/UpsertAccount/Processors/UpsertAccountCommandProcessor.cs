using System.Linq.Expressions;

namespace mark.davison.finance.bff.commands.Scenarios.UpsertAccount.Processors;

public class UpsertAccountCommandProcessor : IUpsertAccountCommandProcessor
{
    private readonly ICommandHandler<CreateTransactionRequest, CreateTransactionResponse> _createTransactionHandler;
    private readonly IDateService _dateService;
    private readonly IRepository _repository;

    public UpsertAccountCommandProcessor(
        ICommandHandler<CreateTransactionRequest, CreateTransactionResponse> createTransactionHandler,
        IDateService dateService,
        IRepository repository
    )
    {
        _dateService = dateService;
        _createTransactionHandler = createTransactionHandler;
        _repository = repository;
    }

    public async Task<UpsertAccountCommandResponse> Process(
        UpsertAccountCommandRequest request,
        UpsertAccountCommandResponse response,
        ICurrentUserContext currentUserContext,
        CancellationToken cancellationToken)
    {

        await using (_repository.BeginTransaction())
        {

            var existingAccount = await _repository.GetEntityAsync<Account>(
                request.UpsertAccountDto.Id,
                cancellationToken);


            var existingOpeningBalanceTransactionJournal = await _repository.GetEntityAsync<TransactionJournal>(
                _ =>
                    _.TransactionTypeId == TransactionConstants.OpeningBalance &&
                    _.Transactions.Any(__ =>
                        __.AccountId == request.UpsertAccountDto.Id),
                new Expression<Func<TransactionJournal, object>>[]
                {
                        _ => _.Transactions!
                },
                cancellationToken);

            Account account;
            if (existingAccount == null)
            {
                account = new Account
                {
                    Id = request.UpsertAccountDto.Id,
                    Created = _dateService.Now,
                    LastModified = _dateService.Now,
                    IsActive = true,
                    Order = -1,
                    UserId = currentUserContext.CurrentUser.Id
                };
            }
            else
            {
                account = existingAccount;
            }

            account.Name = request.UpsertAccountDto.Name;
            account.VirtualBalance = request.UpsertAccountDto.VirtualBalance;
            account.AccountNumber = request.UpsertAccountDto.AccountNumber;
            account.AccountTypeId = request.UpsertAccountDto.AccountTypeId;
            account.CurrencyId = request.UpsertAccountDto.CurrencyId;

            // TODO: handle roll back/repository insert failure
            await _repository.UpsertEntityAsync(
                account,
                cancellationToken);

            bool requestHasOpeningBalance =
                request.UpsertAccountDto.OpeningBalance != null &&
                request.UpsertAccountDto.OpeningBalanceDate != null;

            bool openingBalanceNeedsEditing = requestHasOpeningBalance && existingOpeningBalanceTransactionJournal != null;
            bool openingBalanceNeedsDeleting = !requestHasOpeningBalance && existingOpeningBalanceTransactionJournal != null;

            if (requestHasOpeningBalance && existingOpeningBalanceTransactionJournal == null)
            {
                // TODO: Replace with IDispatcher/ICQRSDispatcher and handle failure...
                await _createTransactionHandler.Handle(new()
                {
                    TransactionTypeId = TransactionConstants.OpeningBalance,
                    Transactions =
                    {
                        new CreateTransactionDto
                        {
                            Id = Guid.NewGuid(),
                            Amount = request.UpsertAccountDto.OpeningBalance!.Value,
                            Description = "Opening balance",
                            CurrencyId = account.CurrencyId,
                            SourceAccountId = Account.OpeningBalance,
                            DestinationAccountId = account.Id,
                            Date = request.UpsertAccountDto.OpeningBalanceDate!.Value
                        }
                    }
                }, currentUserContext, cancellationToken);
            }
            else if (openingBalanceNeedsEditing || openingBalanceNeedsDeleting)
            {

                if (openingBalanceNeedsEditing)
                {
                    var sourceAccountTransaction = existingOpeningBalanceTransactionJournal!
                            .Transactions
                            .First(_ => _.AccountId == Account.OpeningBalance);
                    var destinationAccountTransaction = existingOpeningBalanceTransactionJournal!
                            .Transactions
                            .First(_ => _.AccountId == account.Id);

                    if (sourceAccountTransaction.Amount != -request.UpsertAccountDto.OpeningBalance!.Value ||
                        destinationAccountTransaction.Amount != +request.UpsertAccountDto.OpeningBalance!.Value ||
                        existingOpeningBalanceTransactionJournal!.Date != request.UpsertAccountDto.OpeningBalanceDate!.Value)
                    {
                        sourceAccountTransaction.Amount = -request.UpsertAccountDto.OpeningBalance!.Value;
                        destinationAccountTransaction.Amount = +request.UpsertAccountDto.OpeningBalance!.Value;
                        existingOpeningBalanceTransactionJournal.Date = request.UpsertAccountDto.OpeningBalanceDate!.Value;

                        await _repository.UpsertEntitiesAsync<Transaction>(new()
                        {
                            sourceAccountTransaction,
                            destinationAccountTransaction
                        }, cancellationToken);

                        await _repository.UpsertEntityAsync(
                            existingOpeningBalanceTransactionJournal!,
                            cancellationToken);
                    }

                }
                else if (openingBalanceNeedsDeleting)
                {
                    await _repository.DeleteEntitiesAsync<Transaction>(
                        existingOpeningBalanceTransactionJournal!.Transactions.Select(_ => _.Id).ToList(),
                        cancellationToken);
                    await _repository.DeleteEntityAsync<TransactionJournal>(
                        existingOpeningBalanceTransactionJournal!.Id,
                        cancellationToken);
                    await _repository.DeleteEntityAsync<TransactionGroup>(
                        existingOpeningBalanceTransactionJournal!.TransactionGroupId,
                        cancellationToken);
                }
            }

            return response;
        }
    }
}
