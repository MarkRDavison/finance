using mark.davison.finance.models.dtos.Commands.CreateTransaction;

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

    [TestMethod]
    public async Task Handle_InvokesRepository()
    {
        var command = new TransactionCreateCommand();

        _httpClientRepository
            .Setup(_ => _
                .Post<CreateTransactionResponse, CreateTransactionRequest>(
                    It.IsAny<CreateTransactionRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse())
            .Verifiable();

        await _handler.Handle(command, CancellationToken.None);

        _httpClientRepository
            .Verify(
                _ => _
                    .Post<CreateTransactionResponse, CreateTransactionRequest>(
                        It.IsAny<CreateTransactionRequest>(),
                        It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
