namespace mark.davison.finance.shared.commands.Scenarios.CreateTransaction.Withdrawal;

public class CreateWithdrawalTransactionValidatorStrategy : CreateTransactionValidatorStrategy
{
    protected override Guid TransactionTypeId => TransactionTypeConstants.Withdrawal;
    protected override IEnumerable<Guid> ValidSourceIds => AccountTypeConstants.Assets.Concat(AccountTypeConstants.Liabilities);
    protected override IEnumerable<Guid> ValidDestinationIds => AccountTypeConstants.Expenses.Concat(AccountTypeConstants.Liabilities);
}
