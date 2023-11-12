namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public interface ICreateTransactionValidatorStrategyFactory
{
    ICreateTransactionValidatorStrategy CreateStrategy(Guid transactionTypeId);
}
