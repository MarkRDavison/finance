namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Withdrawal;

[TestClass]
public class CreateWithdrawalTransactionValidatorStrategyTests
{
    private readonly Mock<ICreateTransctionValidationContext> _createTransctionValidationContext;
    private readonly CreateWithdrawalTransactionValidatorStrategy _strategy;

    public CreateWithdrawalTransactionValidatorStrategyTests()
    {
        _createTransctionValidationContext = new(MockBehavior.Strict);
        _strategy = new();
    }

    [TestMethod]
    public async Task ValidateTransactionGroup_DoesNothing()
    {
        var request = new CreateTransactionRequest();
        var response = new CreateTransactionResponse();

        await _strategy.ValidateTransactionGroup(request, response, _createTransctionValidationContext.Object);

        Assert.IsFalse(response.Error.Any());
    }
}
