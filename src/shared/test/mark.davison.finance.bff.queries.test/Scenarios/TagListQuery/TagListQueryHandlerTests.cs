namespace mark.davison.finance.bff.queries.test.Scenarios.TagListQuery;

[TestClass]
public class TagListQueryHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TagListQueryHandler _handler;
    private readonly CancellationToken _token;

    public TagListQueryHandlerTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new TagListQueryHandler((IFinanceDbContext)_dbContext);
    }

    [TestMethod]
    public async Task Handle_RetrievesTagsFromRepository()
    {
        var tags = new List<Tag>
        {
            new Tag{ Id = Guid.NewGuid() },
            new Tag{ Id = Guid.NewGuid() }
        };

        await _dbContext.UpsertEntitiesAsync(tags, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new TagListQueryRequest();
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.SuccessWithValue.Should().BeTrue();
        response.Value.Should().HaveCount(tags.Count);
    }
}
