namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTag;

[TestClass]
public class CreateTagCommandHandlerTests
{
    private readonly Mock<IRepository> _repositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly CreateTagCommandHandler _createTagCommandHandler;
    private readonly Mock<ICreateTagCommandValidator> _createTagCommandValidatorMock;

    public CreateTagCommandHandlerTests()
    {
        _repositoryMock = new(MockBehavior.Strict);
        _currentUserContextMock = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });
        _createTagCommandValidatorMock = new Mock<ICreateTagCommandValidator>(MockBehavior.Strict);

        _createTagCommandHandler = new CreateTagCommandHandler(
            _repositoryMock.Object,
            _createTagCommandValidatorMock.Object);
    }

    [TestMethod]
    public async Task WhenValidationFails_NonSuccessReponseIsReturned()
    {
        var request = new CreateTagCommandRequest { };
        var validatorResponse = new CreateTagCommandResponse
        {
            Success = false
        };

        _createTagCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<CreateTagCommandRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(validatorResponse);

        var response = await _createTagCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.AreEqual(validatorResponse, response);
    }
}
