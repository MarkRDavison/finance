namespace mark.davison.finance.shared.commands.test.Scenarios.CreateCategory;

[TestClass]
public class CreateCategoryCommandProcessorTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;

    private readonly CreateCategoryCommandProcessor _processor;

    public CreateCategoryCommandProcessorTests()
    {
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { }); // TODO: Extract out mocking of current user context


        _processor = new((IFinanceDbContext)_dbContext);
    }

    [TestMethod]
    public async Task ProcessAsync_WhereCategoryCreated_ReturnsSuccess()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "NEW CATEGORY"
        };

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var category = await _dbContext.GetByIdAsync<Category>(request.Id, CancellationToken.None);

        category.Should().NotBeNull();
        category!.Name.Should().BeEquivalentTo(request.Name); // TODO: Custom NotBeNull that uses [NotNull]
    }
}
