namespace mark.davison.finance.shared.commands.Scenarios.CreateTransaction.Common;

public interface ICreateTransactionValidatorStrategyFactory
{
    ICreateTransactionValidatorStrategy CreateStrategy(Guid transactionTypeId);
}
