namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionValidatorStrategyFactory : ICreateTransactionValidatorStrategyFactory
{
    public ICreateTransactionValidatorStrategy CreateStrategy(Guid transactionTypeId)
    {
        if (transactionTypeId == TransactionConstants.Withdrawal)
        {
            return new CreateWithdrawalTransactionValidatorStrategy();
        }
        else if (transactionTypeId == TransactionConstants.Deposit)
        {
            return new CreateDepositTransactionValidatorStrategy();
        }
        else if (transactionTypeId == TransactionConstants.Transfer)
        {
            return new CreateTransferTransactionValidatorStrategy();
        }

        return new NullCreateTransactionValidatorStrategy();
    }
}
