namespace mark.davison.finance.shared.commands.test.Scenarios.CreateTransaction.Common;

[TestClass]
public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<ICommandValidator<CreateTransactionRequest, CreateTransactionResponse>> _validator;
    private readonly Mock<ICommandProcessor<CreateTransactionRequest, CreateTransactionResponse>> _processor;
    private readonly Mock<ICurrentUserContext> _currentUserContext;

    private readonly CreateTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _validator = new(MockBehavior.Strict);
        _processor = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);

        _handler = new(_processor.Object, _validator.Object);

        _validator
            .Setup(_ => _.ValidateAsync(
                It.IsAny<CreateTransactionRequest>(),
                It.IsAny<ICurrentUserContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse());
    }

    [TestMethod]
    public async Task Handle_InvokesValidator()
    {
        var command = new CreateTransactionRequest();

        _validator
            .Setup(_ => _.ValidateAsync(
                command,
                _currentUserContext.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse
            {
                Errors = new() { "ERROR" }
            })
            .Verifiable();

        await _handler.Handle(command, _currentUserContext.Object, CancellationToken.None);

        _validator
            .Verify(
                _ => _.ValidateAsync(
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
            .Setup(_ => _.ProcessAsync(
                command,
                _currentUserContext.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTransactionResponse())
            .Verifiable();

        await _handler.Handle(command, _currentUserContext.Object, CancellationToken.None);

        _processor
            .Verify(
                _ => _.ProcessAsync(
                    command,
                    _currentUserContext.Object,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
