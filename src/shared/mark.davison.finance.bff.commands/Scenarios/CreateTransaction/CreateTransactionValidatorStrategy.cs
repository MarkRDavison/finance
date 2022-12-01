namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction;

public abstract class CreateTransactionValidatorStrategy : ICreateTransactionValidatorStrategy
{
    public const string VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE = "INVALID_DESTINATION_ACCOUNT_TYPE";
    public const string VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE = "INVALID_SOURCE_ACCOUNT_TYPE";
    public const string VALIDATION_INVALID_ACCOUNT_PAIR = "INVALID_ACCOUNT_PAIR";

    public virtual async Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionCommandResponse response, ICreateTransctionValidationContext context)
    {
        var sourceAccount = await context.GetAccountById(transaction.SourceAccountId, CancellationToken.None) ?? throw new NotImplementedException("Need to handle creating new accounts?");
        var destinationAccount = await context.GetAccountById(transaction.DestinationAccountId, CancellationToken.None) ?? throw new NotImplementedException("Need to handle creating new accounts?");

        if (!ValidDestinationIds.Any(_ => _.Equals(destinationAccount.AccountTypeId)))
        {
            response.Error.Add(VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE);
        }

        if (!ValidSourceIds.Any(_ => _.Equals(sourceAccount.AccountTypeId)))
        {
            response.Error.Add(VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE);
        }

        if (!AllowableSourceDestinationAccounts.IsValidSourceAndDestinationAccount(
            TransactionTypeId,
            sourceAccount.AccountTypeId,
            destinationAccount.AccountTypeId))
        {
            response.Error.Add(VALIDATION_INVALID_ACCOUNT_PAIR);
        }
        // TODO: Validate that no duplicate tags are passed, warning?
    }

    public virtual Task ValidateTransactionGroup(CreateTransactionCommandRequest request, CreateTransactionCommandResponse response, ICreateTransctionValidationContext context)
    {
        return Task.FromResult(context);
    }

    protected abstract Guid TransactionTypeId { get; }
    protected abstract IEnumerable<Guid> ValidSourceIds { get; }
    protected abstract IEnumerable<Guid> ValidDestinationIds { get; }
}
