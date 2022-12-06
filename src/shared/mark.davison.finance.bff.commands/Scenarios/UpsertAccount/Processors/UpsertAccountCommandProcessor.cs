namespace mark.davison.finance.bff.commands.Scenarios.UpsertAccount.Processors;

public class UpsertAccountCommandProcessor : IUpsertAccountCommandProcessor
{
    private readonly ICommandHandler<CreateTransactionCommandRequest, CreateTransactionCommandResponse> _createTransactionHandler;
    private readonly IDateService _dateService;

    public UpsertAccountCommandProcessor(
        ICommandHandler<CreateTransactionCommandRequest, CreateTransactionCommandResponse> createTransactionHandler,
        IDateService dateService
    )
    {
        _dateService = dateService;
        _createTransactionHandler = createTransactionHandler;
    }

    public async Task<UpsertAccountCommandResponse> Process(
        UpsertAccountCommandRequest request,
        UpsertAccountCommandResponse response,
        ICurrentUserContext currentUserContext,
        IHttpRepository httpRepository,
        CancellationToken cancellationToken)
    {
        var headerParameters = HeaderParameters.Auth(
                currentUserContext.Token,
                currentUserContext.CurrentUser);

        var existingAccount = await httpRepository.GetEntityAsync<Account>(
            request.UpsertAccountDto.Id,
            headerParameters,
            cancellationToken);

        var transactionQuery = new QueryParameters();
        transactionQuery.Add(nameof(Transaction.AccountId), request.UpsertAccountDto.Id.ToString());
        transactionQuery.Add(string.Format("{0}.{1}",
                nameof(Transaction.TransactionJournal),
                nameof(TransactionJournal.TransactionTypeId)
            ), TransactionConstants.OpeningBalance.ToString());
        var existingOpeningBalance = await httpRepository.GetEntityAsync<Transaction>(
            transactionQuery,
            headerParameters,
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

        await httpRepository.UpsertEntityAsync(
            account,
            headerParameters,
            cancellationToken);

        bool requestHasOpeningBalance =
            request.UpsertAccountDto.OpeningBalance != null &&
            request.UpsertAccountDto.OpeningBalanceDate != null;

        bool openingBalanceNeedsEditing = requestHasOpeningBalance && existingOpeningBalance != null;
        bool openingBalanceNeedsDeleting = !requestHasOpeningBalance && existingOpeningBalance != null;

        if (requestHasOpeningBalance && existingOpeningBalance == null)
        {
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
            var transactionJournalQuery = new QueryParameters();
            transactionJournalQuery.Add(nameof(TransactionJournal.Id), existingOpeningBalance!.TransactionJournalId.ToString());
            transactionJournalQuery.Include(nameof(TransactionJournal.Transactions));
            var transactionJournal = await httpRepository.GetEntityAsync<TransactionJournal>(
                transactionJournalQuery,
                headerParameters,
                cancellationToken);

            if (openingBalanceNeedsEditing)
            {
                var sourceAccountTransaction = transactionJournal!
                        .Transactions
                        .First(_ => _.AccountId == Account.OpeningBalance);
                var destinationAccountTransaction = transactionJournal!
                        .Transactions
                        .First(_ => _.AccountId == account.Id);

                if (sourceAccountTransaction.Amount != -request.UpsertAccountDto.OpeningBalance!.Value ||
                    destinationAccountTransaction.Amount != +request.UpsertAccountDto.OpeningBalance!.Value ||
                    transactionJournal!.Date != request.UpsertAccountDto.OpeningBalanceDate!.Value)
                {
                    sourceAccountTransaction.Amount = -request.UpsertAccountDto.OpeningBalance!.Value;
                    destinationAccountTransaction.Amount = +request.UpsertAccountDto.OpeningBalance!.Value;
                    transactionJournal.Date = request.UpsertAccountDto.OpeningBalanceDate!.Value;

                    await httpRepository.UpsertEntitiesAsync<Transaction>(new()
            {
                sourceAccountTransaction,
                destinationAccountTransaction
            }, headerParameters, cancellationToken);

                    await httpRepository.UpsertEntityAsync(
                        transactionJournal!,
                        headerParameters,
                        cancellationToken);
                }

            }
            else if (openingBalanceNeedsDeleting)
            {
                await httpRepository.DeleteEntityAsync<TransactionJournal>(
                    transactionJournal!.Id,
                    headerParameters,
                    cancellationToken);
                await httpRepository.DeleteEntityAsync<TransactionGroup>(
                    transactionJournal!.TransactionGroupId,
                    headerParameters,
                    cancellationToken);
                await httpRepository.DeleteEntitiesAsync<Transaction>(
                    transactionJournal!.Transactions.Select(_ => _.Id).ToList(),
                    headerParameters,
                    cancellationToken);
            }
        }

        return response;
    }
}
