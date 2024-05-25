namespace mark.davison.finance.shared.commands.Scenarios.UpsertAccount.Processors;

public class UpsertAccountCommandProcessor : ICommandProcessor<UpsertAccountCommandRequest, UpsertAccountCommandResponse>
{
    private readonly ICommandHandler<CreateTransactionRequest, CreateTransactionResponse> _createTransactionHandler;
    private readonly IDateService _dateService;
    private readonly IFinanceDbContext _dbContext;

    public UpsertAccountCommandProcessor(
        ICommandHandler<CreateTransactionRequest, CreateTransactionResponse> createTransactionHandler,
        IDateService dateService,
        IFinanceDbContext dbContext
    )
    {
        _dateService = dateService;
        _createTransactionHandler = createTransactionHandler;
        _dbContext = dbContext;
    }

    public async Task<UpsertAccountCommandResponse> ProcessAsync(
        UpsertAccountCommandRequest request,
        ICurrentUserContext currentUserContext,
        CancellationToken cancellationToken)
    {
        using (var transaction = await _dbContext.BeginTransactionAsync(cancellationToken))
        {
            var existingAccount = await _dbContext.GetByIdAsync<Account>(
                request.UpsertAccountDto.Id,
                cancellationToken);

            var existingOpeningBalanceTransactionJournal = await _dbContext
                .Set<TransactionJournal>()
                .Include(_ => _.Transactions)
                .Where(_ =>
                    _.TransactionTypeId == TransactionTypeConstants.OpeningBalance &&
                    _.Transactions.Any(__ =>
                        __.AccountId == request.UpsertAccountDto.Id))
                .FirstOrDefaultAsync(cancellationToken);

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

            await _dbContext.UpsertEntityAsync(account, cancellationToken);

            bool requestHasOpeningBalance =
                request.UpsertAccountDto.OpeningBalance != null &&
                request.UpsertAccountDto.OpeningBalanceDate != null;

            bool openingBalanceNeedsEditing = requestHasOpeningBalance && existingOpeningBalanceTransactionJournal != null;
            bool openingBalanceNeedsDeleting = !requestHasOpeningBalance && existingOpeningBalanceTransactionJournal != null;

            if (requestHasOpeningBalance && existingOpeningBalanceTransactionJournal == null)
            {
                // TODO: _dbContext transaction
                // TODO: Replace with IDispatcher/ICQRSDispatcher and handle failure...
                await _createTransactionHandler.Handle(new()
                {
                    TransactionTypeId = TransactionTypeConstants.OpeningBalance,
                    Transactions =
                {
                    new CreateTransactionDto
                    {
                        Id = Guid.NewGuid(),
                        Amount = request.UpsertAccountDto.OpeningBalance!.Value,
                        Description = "Opening balance", // TODO: Constant
                        CurrencyId = account.CurrencyId,
                        SourceAccountId = AccountConstants.OpeningBalance,
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
                            .First(_ => _.AccountId == AccountConstants.OpeningBalance);
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

                        await _dbContext.UpsertEntitiesAsync<Transaction>(
                        [
                            sourceAccountTransaction,
                            destinationAccountTransaction
                        ], cancellationToken);

                        await _dbContext.UpsertEntityAsync(
                            existingOpeningBalanceTransactionJournal!,
                            cancellationToken);
                    }

                }
                else if (openingBalanceNeedsDeleting)
                {
                    await _dbContext.DeleteEntitiesByIdAsync<Transaction>(
                        existingOpeningBalanceTransactionJournal!.Transactions.Select(_ => _.Id).ToList(),
                        cancellationToken);
                    await _dbContext.DeleteEntityByIdAsync<TransactionJournal>(
                        existingOpeningBalanceTransactionJournal!.Id,
                        cancellationToken);
                    await _dbContext.DeleteEntityByIdAsync<TransactionGroup>(
                        existingOpeningBalanceTransactionJournal!.TransactionGroupId,
                        cancellationToken);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitTransactionAsync(cancellationToken);

            var response = new UpsertAccountCommandResponse
            {
                Value = new() // TODO: Helper
                {
                    Id = account.Id,
                    Name = account.Name,
                    AccountNumber = account.AccountNumber,
                    // AccountType = account.AccountType!.Type, TODO: Remove, client can look it up
                    AccountTypeId = account.AccountTypeId,
                    Active = account.IsActive,
                    CurrencyId = account.CurrencyId,
                    LastModified = account.LastModified,
                    VirtualBalance = account.VirtualBalance,
                    OpeningBalance = request.UpsertAccountDto.OpeningBalance,
                    OpeningBalanceDate = request.UpsertAccountDto.OpeningBalanceDate,
                    BalanceDifference = request.UpsertAccountDto.OpeningBalance ?? 0,
                    CurrentBalance = request.UpsertAccountDto.OpeningBalance ?? 0
                }
            };

            return response;
        }
    }
}
