namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTag;

[TestClass]
public class CreateTagCommandHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly CreateTagCommandHandler _createTagCommandHandler;
    private readonly Mock<ICreateTagCommandValidator> _createTagCommandValidatorMock;

    public CreateTagCommandHandlerTests()
    {
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));
        _currentUserContextMock = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });
        _createTagCommandValidatorMock = new Mock<ICreateTagCommandValidator>(MockBehavior.Strict);

        _createTagCommandHandler = new CreateTagCommandHandler(
            (IFinanceDbContext)_dbContext,
            _createTagCommandValidatorMock.Object);
    }

    [TestMethod]
    public async Task WhenValidationFails_NonSuccessReponseIsReturned()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "TAG_NAME"
        };
        var validatorResponse = new CreateTagCommandResponse();

        _createTagCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<CreateTagCommandRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(validatorResponse);

        var response = await _createTagCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        response.Success.Should().BeFalse();

    }

    [TestMethod]
    public async Task WhenValidationPasses_SuccessReponseIsReturned()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "TAG_NAME"
        };
        var validatorResponse = new CreateTagCommandResponse();

        _createTagCommandValidatorMock
            .Setup(_ => _.Validate(
                It.IsAny<CreateTagCommandRequest>(),
                _currentUserContextMock.Object,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(validatorResponse);

        var response = await _createTagCommandHandler.Handle(request, _currentUserContextMock.Object, CancellationToken.None);

        response.Success.Should().BeTrue();

        var tag = await _dbContext.GetByIdAsync<Tag>(request.Id, CancellationToken.None);

        tag.Should().NotBeNull();
        tag!.Name.Should().Be(request.Name);
    }
}
