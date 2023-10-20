namespace mark.davison.finance.bff.commands.test.Scenarios.UpsertAccount;

[TestClass]
public class UpsertAccountCommandHandlerTests
{
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly UpsertAccountCommandHandler _upsertAccountCommandHandler;
    private readonly Mock<IUpsertAccountCommandValidator> _upsertAccountCommandValidatorMock;
    private readonly Mock<IUpsertAccountCommandProcessor> _upsertAccountCommandProcessorMock;

    public UpsertAccountCommandHandlerTests()
    {
        _currentUserContextMock = new(MockBehavior.Strict);
        _upsertAccountCommandProcessorMock = new(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });
        _upsertAccountCommandValidatorMock = new Mock<IUpsertAccountCommandValidator>(MockBehavior.Strict);

        _upsertAccountCommandHandler = new UpsertAccountCommandHandler(
            _upsertAccountCommandValidatorMock.Object,
            _upsertAccountCommandProcessorMock.Object);

        _upsertAccountCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<UpsertAccountCommandRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UpsertAccountCommandResponse
            {
                Success = true
            });
    }

    [TestMethod]
    public async Task WhenValidationFails_NonSuccessReponseIsReturned()
    {
        var request = new UpsertAccountCommandRequest { };
        var validatorResponse = new UpsertAccountCommandResponse
        {
            Success = false
        };

        _upsertAccountCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<UpsertAccountCommandRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatorResponse);

        var response = await _upsertAccountCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        Assert.IsFalse(response.Success);
        Assert.AreEqual(validatorResponse, response);
    }
}

