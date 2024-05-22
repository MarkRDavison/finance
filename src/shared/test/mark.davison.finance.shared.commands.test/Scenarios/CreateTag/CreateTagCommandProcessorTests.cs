namespace mark.davison.finance.shared.commands.test.Scenarios.CreateTag;

[TestClass]
public class CreateTagCommandProcessorTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly CreateTagCommandProcessor _createTagCommandProcessor;

    public CreateTagCommandProcessorTests()
    {
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));
        _currentUserContextMock = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContextMock.Setup(_ => _.Token).Returns("");
        _currentUserContextMock.Setup(_ => _.CurrentUser).Returns(new User { });

        _createTagCommandProcessor = new CreateTagCommandProcessor((IFinanceDbContext)_dbContext);
    }

    [TestMethod]
    public async Task WhenValidationPasses_SuccessReponseIsReturned()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "TAG_NAME"
        };

        var response = await _createTagCommandProcessor.ProcessAsync(request, _currentUserContextMock.Object, CancellationToken.None);

        response.Success.Should().BeTrue();

        var tag = await _dbContext.GetByIdAsync<Tag>(request.Id, CancellationToken.None);

        tag.Should().NotBeNull();
        tag!.Id.Should().Be(request.Id);
        tag!.Name.Should().Be(request.Name);
    }
}
