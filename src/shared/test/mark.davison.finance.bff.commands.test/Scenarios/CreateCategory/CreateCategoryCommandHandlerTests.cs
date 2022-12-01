namespace mark.davison.finance.bff.commands.test.Scenarios.CreateCategory;

[TestClass]
public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly CreateCategoryCommandHandler _createCategoryCommandHandler;
    private readonly Mock<ICreateCategoryCommandValidator> _createCategoryCommandValidatorMock;

    public CreateCategoryCommandHandlerTests()
    {
        _httpRepositoryMock = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContextMock = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });
        _createCategoryCommandValidatorMock = new Mock<ICreateCategoryCommandValidator>(MockBehavior.Strict);

        _createCategoryCommandHandler = new CreateCategoryCommandHandler(
            _httpRepositoryMock.Object,
            _createCategoryCommandValidatorMock.Object);
    }

    [TestMethod]
    public async Task WhenValidationFails_NonSuccessReponseIsReturned()
    {
        var request = new CreateCategoryCommandRequest { };
        var validatorResponse = new CreateCategoryCommandResponse
        {
            Success = false
        };

        _createCategoryCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<CreateCategoryCommandRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(validatorResponse);

        var response = await _createCategoryCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.AreEqual(validatorResponse, response);
    }
}
