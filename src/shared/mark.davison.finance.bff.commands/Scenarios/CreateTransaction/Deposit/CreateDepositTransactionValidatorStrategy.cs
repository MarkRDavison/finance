namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Deposit;

public class CreateDepositTransactionValidatorStrategy : ICreateTransactionValidatorStrategy
{
    public const string VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE = "INVALID_DESTINATION_ACCOUNT_TYPE";
    public const string VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE = "INVALID_SOURCE_ACCOUNT_TYPE";
    public const string VALIDATION_INVALID_ACCOUNT_PAIR = "INVALID_ACCOUNT_PAIR";

    public async Task ValidateTranasction(CreateTransactionDto transaction, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        var sourceAccount = await context.GetAccountById(transaction.SourceAccountId, CancellationToken.None) ?? throw new NotImplementedException("Need to handle creating new accounts?");
        var destinationAccount = await context.GetAccountById(transaction.DestinationAccountId, CancellationToken.None) ?? throw new NotImplementedException("Need to handle creating new accounts?");

        var validDestinationIds = AccountConstants.Assets.Concat(AccountConstants.Liabilities);
        if (!validDestinationIds.Any(_ => _.Equals(destinationAccount.AccountTypeId)))
        {
            response.Error.Add(VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE);
        }

        var validSourceIds = AccountConstants.Revenues;
        if (!validSourceIds.Any(_ => _.Equals(sourceAccount.AccountTypeId)))
        {
            response.Error.Add(VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE);
        }

        if (!AllowableSourceDestinationAccounts.IsValidSourceAndDestinationAccount(
            TransactionConstants.Deposit,
            sourceAccount.AccountTypeId,
            destinationAccount.AccountTypeId))
        {
            response.Error.Add(VALIDATION_INVALID_ACCOUNT_PAIR);
        }
    }

    public Task ValidateTransactionGroup(CreateTransactionRequest request, CreateTransactionResponse response, ICreateTransctionValidationContext context)
    {
        return Task.CompletedTask;
    }
}
