namespace mark.davison.finance.shared.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionValidatorStrategyFactory : ICreateTransactionValidatorStrategyFactory
{
    public ICreateTransactionValidatorStrategy CreateStrategy(Guid transactionTypeId)
    {
        if (transactionTypeId == TransactionTypeConstants.Withdrawal)
        {
            return new CreateWithdrawalTransactionValidatorStrategy();
        }
        else if (transactionTypeId == TransactionTypeConstants.Deposit)
        {
            return new CreateDepositTransactionValidatorStrategy();
        }
        else if (transactionTypeId == TransactionTypeConstants.Transfer)
        {
            return new CreateTransferTransactionValidatorStrategy();
        }

        return new NullCreateTransactionValidatorStrategy();
    }
}
