﻿namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Withdrawal;

public class CreateWithdrawalTransactionValidatorStrategy : CreateTransactionValidatorStrategy
{
    protected override Guid TransactionTypeId => TransactionConstants.Withdrawal;
    protected override IEnumerable<Guid> ValidSourceIds => AccountTypeConstants.Assets.Concat(AccountTypeConstants.Liabilities);
    protected override IEnumerable<Guid> ValidDestinationIds => AccountTypeConstants.Expenses.Concat(AccountTypeConstants.Liabilities);
}
