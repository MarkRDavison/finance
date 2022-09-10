namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Withdrawal;

public class CreateWithdrawalTransactionValidatorStrategy : CreateTransactionValidatorStrategy
{
    protected override Guid TransactionTypeId => TransactionConstants.Withdrawal;
    protected override IEnumerable<Guid> ValidSourceIds => AccountConstants.Assets.Concat(AccountConstants.Liabilities);
    protected override IEnumerable<Guid> ValidDestinationIds => AccountConstants.Expenses.Concat(AccountConstants.Liabilities);
}
