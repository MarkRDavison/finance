using mark.davison.finance.common.server.abstractions.Repository;

namespace mark.davison.finance.bff.commands.test.Scenarios.CreateAccount;

[TestClass]
public class CreateAccountCommandHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly CreateAccountCommandHandler _createAccountCommandHandler;
    private readonly Mock<ICreateAccountCommandValidator> _createAccountCommandValidatorMock;

    public CreateAccountCommandHandlerTests()
    {
        _httpRepositoryMock = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContextMock = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });
        _createAccountCommandValidatorMock = new Mock<ICreateAccountCommandValidator>(MockBehavior.Strict);

        _createAccountCommandHandler = new CreateAccountCommandHandler(
            _httpRepositoryMock.Object,
            _createAccountCommandValidatorMock.Object);
    }

    [TestMethod]
    public async Task WhenValidationFails_NonSuccessReponseIsReturned()
    {
        var request = new CreateAccountRequest { };
        var validatorResponse = new CreateAccountResponse
        {
            Success = false
        };

        _createAccountCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<CreateAccountRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatorResponse);

        var response = await _createAccountCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.AreEqual(validatorResponse, response);
    }
}

