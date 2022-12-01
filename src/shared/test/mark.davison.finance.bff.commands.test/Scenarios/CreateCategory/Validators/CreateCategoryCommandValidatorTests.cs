namespace mark.davison.finance.bff.commands.test.Scenarios.CreateCategory.Validators;

[TestClass]
public class CreateCategoryCommandValidatorTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateCategoryCommandValidator _validator;

    public CreateCategoryCommandValidatorTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _validator = new(_httpRepository.Object);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });
    }

    [TestMethod]
    public async Task Validate_Passes_WhenNoDuplicateCategoryExists()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        _httpRepository
            .Setup(_ => _.GetEntityAsync<Category>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null)
            .Verifiable();

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        _httpRepository
            .Verify(_ =>
                _.GetEntityAsync<Category>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.Error.Any());
    }

    [TestMethod]
    public async Task Validate_Fails_WhenDuplicateCategoryExistsForCurrentUser()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        _httpRepository
            .Setup(_ => _.GetEntityAsync<Category>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                Assert.IsTrue(q.ContainsKey(nameof(Category.Name)));
                Assert.IsTrue(q.ContainsKey(nameof(Category.UserId)));

                return new Category();
            })
            .Verifiable();

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        _httpRepository
            .Verify(_ =>
                _.GetEntityAsync<Category>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Any(_ => _ == CreateCategoryCommandValidator.VALIDATION_DUPLICATE_CATEGORY_NAME));
    }

    [TestMethod]
    public async Task Validate_Passes_WhenDuplicateCategoryExistsForDifferentUser()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Category Name"
        };

        _httpRepository
            .Setup(_ => _.GetEntityAsync<Category>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                Assert.IsTrue(q.ContainsKey(nameof(Category.Name)));
                Assert.IsTrue(q.ContainsKey(nameof(Category.UserId)));

                return null;
            })
            .Verifiable();

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        _httpRepository
            .Verify(_ =>
                _.GetEntityAsync<Category>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.Error.Any(_ => _ == CreateCategoryCommandValidator.VALIDATION_DUPLICATE_CATEGORY_NAME));
    }
}
