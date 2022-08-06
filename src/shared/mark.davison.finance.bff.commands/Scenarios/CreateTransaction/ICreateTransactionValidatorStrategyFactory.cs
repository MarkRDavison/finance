namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction;

public interface ICreateTransactionValidatorStrategyFactory
{
    ICreateTransactionValidatorStrategy CreateStrategy(Guid transactionTypeId);
}
