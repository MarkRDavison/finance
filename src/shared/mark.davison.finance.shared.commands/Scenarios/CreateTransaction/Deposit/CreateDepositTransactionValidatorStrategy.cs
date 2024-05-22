namespace mark.davison.finance.shared.commands.Scenarios.CreateTransaction.Deposit;

public class CreateDepositTransactionValidatorStrategy : CreateTransactionValidatorStrategy
{
    protected override Guid TransactionTypeId => TransactionConstants.Deposit;
    protected override IEnumerable<Guid> ValidSourceIds => AccountTypeConstants.Revenues;
    protected override IEnumerable<Guid> ValidDestinationIds => AccountTypeConstants.Assets.Concat(AccountTypeConstants.Liabilities);
}
