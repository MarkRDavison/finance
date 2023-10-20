namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTag.Validators;

[TestClass]
public class CreateTagCommandValidatorTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateTagCommandValidator _validator;

    public CreateTagCommandValidatorTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _validator = new(_repository.Object);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });
    }

    [TestMethod]
    public async Task Validate_Passes_WhenNoDuplicateTagExists()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Tag Name"
        };

        _repository
            .Setup(_ => _.GetEntityAsync<Tag>(
                It.IsAny<Expression<Func<Tag, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null)
            .Verifiable();

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(_ =>
                _.GetEntityAsync<Tag>(
                    It.IsAny<Expression<Func<Tag, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.Error.Any());
    }

    [TestMethod]
    public async Task Validate_Fails_WhenDuplicateTagExistsForCurrentUser()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Tag Name"
        };

        _repository
            .Setup(_ => _.GetEntityAsync<Tag>(
                It.IsAny<Expression<Func<Tag, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tag())
            .Verifiable();

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(_ =>
                _.GetEntityAsync<Tag>(
                    It.IsAny<Expression<Func<Tag, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsFalse(response.Success);
        Assert.IsTrue(response.Error.Any(_ => _ == CreateTagCommandValidator.VALIDATION_DUPLICATE_TAG_NAME));
    }

    [TestMethod]
    public async Task Validate_Passes_WhenDuplicateTagExistsForDifferentUser()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Tag Name"
        };

        _repository
            .Setup(_ => _.GetEntityAsync<Tag>(
                It.IsAny<Expression<Func<Tag, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null)
            .Verifiable();

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(_ =>
                _.GetEntityAsync<Tag>(
                    It.IsAny<Expression<Func<Tag, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsTrue(response.Success);
        Assert.IsFalse(response.Error.Any(_ => _ == CreateTagCommandValidator.VALIDATION_DUPLICATE_TAG_NAME));
    }
}
