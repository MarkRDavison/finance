namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Deposit;

public class CreateDepositTransactionValidatorStrategy : CreateTransactionValidatorStrategy
{
    protected override Guid TransactionTypeId => TransactionConstants.Deposit;
    protected override IEnumerable<Guid> ValidSourceIds => AccountConstants.Revenues;
    protected override IEnumerable<Guid> ValidDestinationIds => AccountConstants.Assets.Concat(AccountConstants.Liabilities);
}
