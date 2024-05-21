﻿namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Transfer;

public class CreateTransferTransactionValidatorStrategy : CreateTransactionValidatorStrategy
{
    protected override Guid TransactionTypeId => TransactionConstants.Transfer;
    protected override IEnumerable<Guid> ValidSourceIds => AccountTypeConstants.Assets.Concat(AccountTypeConstants.Liabilities);
    protected override IEnumerable<Guid> ValidDestinationIds => AccountTypeConstants.Assets.Concat(AccountTypeConstants.Liabilities);
}
