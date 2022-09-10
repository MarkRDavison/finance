namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Transfer;

public class CreateTransferTransactionValidatorStrategy : CreateTransactionValidatorStrategy
{
    protected override Guid TransactionTypeId => TransactionConstants.Transfer;
    protected override IEnumerable<Guid> ValidSourceIds => AccountConstants.Assets.Concat(AccountConstants.Liabilities);
    protected override IEnumerable<Guid> ValidDestinationIds => AccountConstants.Assets.Concat(AccountConstants.Liabilities);
}
