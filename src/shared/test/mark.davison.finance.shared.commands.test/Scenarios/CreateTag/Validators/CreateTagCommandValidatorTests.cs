namespace mark.davison.finance.shared.commands.test.Scenarios.CreateTag.Validators;

[TestClass]
public class CreateTagCommandValidatorTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateTagCommandValidator _validator;
    private readonly Guid _userId;

    public CreateTagCommandValidatorTests()
    {
        _userId = Guid.NewGuid();
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));
        _currentUserContext = new(MockBehavior.Strict);
        _validator = new((IFinanceDbContext)_dbContext);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { Id = _userId });
    }
    [TestMethod]
    public async Task Validate_Passes_WhenNoDuplicateTagExists()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Tag Name"
        };

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();
        response.Errors.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Validate_Fails_WhenDuplicateTagExistsForCurrentUser()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Tag Name"
        };

        await _dbContext.UpsertEntityAsync(new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = _userId
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeFalse();
        response.Errors.Should().ContainMatch(CreateTagCommandValidator.VALIDATION_DUPLICATE_TAG_NAME);
    }

    [TestMethod]
    public async Task Validate_Passes_WhenDuplicateTagExistsForDifferentUser()
    {
        var request = new CreateTagCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Tag Name"
        };

        await _dbContext.UpsertEntityAsync(new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = Guid.NewGuid()
        }, CancellationToken.None);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var response = await _validator.Validate(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();
        response.Errors.Should().BeEmpty();
    }
}
