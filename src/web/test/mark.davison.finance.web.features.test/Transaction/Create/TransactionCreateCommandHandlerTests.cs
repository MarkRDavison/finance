namespace mark.davison.finance.web.features.test.Transaction.Create;

[TestClass]
public class TransactionCreateCommandHandlerTests
{
    private readonly Mock<IClientHttpRepository> _httpClientRepository;
    private readonly TransactionCreateCommandHandler _handler;

    public TransactionCreateCommandHandlerTests()
    {
        _httpClientRepository = new(MockBehavior.Strict);
        _handler = new(_httpClientRepository.Object);
    }
}
