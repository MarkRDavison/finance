namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction;

[TestClass]
public class CreateTransactionValidatorStrategyFactoryTests
{
    private readonly CreateTransactionValidatorStrategyFactory _factory;

    public CreateTransactionValidatorStrategyFactoryTests()
    {
        _factory = new();
    }

    [TestMethod]
    public void CreateStrategy_UsingWithdrawalTransactionTypeId_CreatesWithdrawalStrategy()
    {
        var strategy = _factory.CreateStrategy(TransactionConstants.Withdrawal);

        Assert.IsInstanceOfType(strategy, typeof(CreateWithdrawalTransactionValidatorStrategy));
    }

    [TestMethod]
    public void CreateStrategy_UsingDepositTransactionTypeId_CreatesDepositStrategy()
    {
        var strategy = _factory.CreateStrategy(TransactionConstants.Deposit);

        Assert.IsInstanceOfType(strategy, typeof(CreateDepositTransactionValidatorStrategy));
    }

    [TestMethod]
    public void CreateStrategy_UsingTransferTransactionTypeId_CreatesTransferStrategy()
    {
        var strategy = _factory.CreateStrategy(TransactionConstants.Transfer);

        Assert.IsInstanceOfType(strategy, typeof(CreateTransferTransactionValidatorStrategy));
    }

    [TestMethod]
    public void CreateStrategy_UsingInvalidTransactionTypeId_CreatesNullStrategy()
    {
        var strategy = _factory.CreateStrategy(Guid.Empty);

        Assert.IsInstanceOfType(strategy, typeof(NullCreateTransactionValidatorStrategy));
    }
}
