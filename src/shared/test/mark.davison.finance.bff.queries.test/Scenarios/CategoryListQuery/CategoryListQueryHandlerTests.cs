namespace mark.davison.finance.bff.queries.test.Scenarios.CategoryListQuery;

[TestClass]
public class CategoryListQueryHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CategoryListQueryHandler _handler;
    private readonly CancellationToken _token;

    public CategoryListQueryHandlerTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new CategoryListQueryHandler((IFinanceDbContext)_dbContext);
    }

    [TestMethod]
    public async Task Handle_RetrievesCategoriesFromRepository()
    {
        var categories = new List<Category>
        {
            new Category{ Id = Guid.NewGuid(), UserId = _currentUserContext.Object.CurrentUser.Id },
            new Category{ Id = Guid.NewGuid(), UserId = _currentUserContext.Object.CurrentUser.Id }
        };

        await _dbContext.UpsertEntitiesAsync(categories, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new CategoryListQueryRequest();
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.Categories.Should().HaveCount(categories.Count);
    }
}

