using mark.davison.finance.models.dtos.Commands.CreateTransaction;

namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Common;

[TestClass]
public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICreateTransactionCommandValidator> _validator;
    private readonly Mock<ICreateTransactionCommandProcessor> _processor;
    private readonly Mock<ICurrentUserContext> _currentUserContext;

    private readonly CreateTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _validator = new(MockBehavior.Strict);
        _processor = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);

        _handler = new(_httpRepository.Object, _validator.Object, _processor.Object);

        _validator
            .Setup(_ => _.Validate(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse
            {
                Success = true
            });
    }

    [TestMethod]
    public async Task Handle_InvokesValidator()
    {
        var command = new CreateTransactionRequest();

        _validator
            .Setup(_ => _.Validate(
                command,
                _currentUserContext.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse
            {
                Success = false
            })
            .Verifiable();

        await _handler.Handle(command, _currentUserContext.Object, CancellationToken.None);

        _validator
            .Verify(
                _ => _.Validate(
                    command,
                    _currentUserContext.Object,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Handle_InvokesProcessor_WhenValidationPasses()
    {
        var command = new CreateTransactionRequest();

        _processor
            .Setup(_ => _.Process(
                command,
                It.IsAny<CreateTransactionResponse>(),
                _currentUserContext.Object,
                _httpRepository.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse())
            .Verifiable();

        await _handler.Handle(command, _currentUserContext.Object, CancellationToken.None);

        _processor
            .Verify(
                _ => _.Process(
                    command,
                    It.IsAny<CreateTransactionResponse>(),
                    _currentUserContext.Object,
                _httpRepository.Object,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
