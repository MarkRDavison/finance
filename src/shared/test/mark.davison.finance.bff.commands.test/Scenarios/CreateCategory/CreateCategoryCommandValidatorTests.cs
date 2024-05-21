namespace mark.davison.finance.bff.commands.test.Scenarios.CreateCategory;

[TestClass]
public class CreateCategoryCommandValidatorTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateCategoryCommandValidator _validator;
    private readonly Guid _userId;

    public CreateCategoryCommandValidatorTests()
    {
        _userId = Guid.NewGuid();
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));
        _currentUserContext = new(MockBehavior.Strict);
        _validator = new((IFinanceDbContext)_dbContext);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { Id = _userId }); ;

    }

    [TestMethod]
    public async Task Validate_Passes_WhenNoDuplicateCategoryExists()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();
        response.Errors.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Validate_Fails_WhenDuplicateCategoryExistsForCurrentUser()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        await _dbContext.UpsertEntityAsync(new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = _userId
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch(CreateCategoryCommandValidator.VALIDATION_DUPLICATE_CATEGORY_NAME);
    }

    [TestMethod]
    public async Task Validate_Passes_WhenDuplicateCategoryExistsForDifferentUser()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        await _dbContext.UpsertEntityAsync(new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = Guid.NewGuid()
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();
        response.Errors.Should().BeEmpty();
    }
}
