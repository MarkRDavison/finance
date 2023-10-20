namespace mark.davison.finance.bff.commands.test.Scenarios.CreateCategory;

[TestClass]
public class CreateCategoryCommandValidatorTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _validator = new(_repository.Object);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });
        _repository.Setup(_ => _.BeginTransaction()).Returns(() => new TestAsyncDisposable());

    }

    [TestMethod]
    public async Task Validate_Passes_WhenNoDuplicateCategoryExists()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        _repository
            .Setup(_ => _.EntityExistsAsync<Category>(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .Verifiable();

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(_ =>
                _.EntityExistsAsync<Category>(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.Errors.Any());
    }

    [TestMethod]
    public async Task Validate_Fails_WhenDuplicateCategoryExistsForCurrentUser()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        _repository
            .Setup(_ => _.EntityExistsAsync<Category>(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(_ =>
                _.EntityExistsAsync<Category>(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Errors.Any(_ => _ == CreateCategoryCommandValidator.VALIDATION_DUPLICATE_CATEGORY_NAME));
    }

    [TestMethod]
    public async Task Validate_Passes_WhenDuplicateCategoryExistsForDifferentUser()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        _repository
            .Setup(_ => _.EntityExistsAsync<Category>(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .Verifiable();

        var response = await _validator.ValidateAsync(request, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(_ =>
                _.EntityExistsAsync<Category>(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.Errors.Any(_ => _ == CreateCategoryCommandValidator.VALIDATION_DUPLICATE_CATEGORY_NAME));
    }
}
