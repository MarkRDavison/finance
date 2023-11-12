namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Common.Validators;

[TestClass]
public class CreateTransctionValidationContextTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateTransctionValidationContext _context;

    public CreateTransctionValidationContextTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _context = new(_repository.Object, _currentUserContext.Object);

        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User());

        _repository.Setup(_ => _.BeginTransaction()).Returns(() => new TestAsyncDisposable());

    }

    [TestMethod]
    public async Task GetAccountById_FetchesFromRepository()
    {
        var accountId = Guid.NewGuid();

        _repository.Setup(_ => _.GetEntityAsync<Account>(
                accountId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Account
            {
                Id = accountId
            })
            .Verifiable();

        var first = await _context.GetAccountById(accountId, CancellationToken.None);

        Assert.IsNotNull(first);

        _repository
            .Verify(
                _ => _.GetEntityAsync<Account>(
                    accountId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task GetAccountById_DoesNotFetchFromRepositoryTwice()
    {
        var accountId = Guid.NewGuid();


        _repository.Setup(_ => _.GetEntityAsync<Account>(
                accountId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Account
            {
                Id = accountId
            })
            .Verifiable();

        await _context.GetAccountById(accountId, CancellationToken.None);

        _repository
            .Verify(
                _ => _.GetEntityAsync<Account>(
                    accountId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task GetCategoryById_FetchesFromRepository()
    {
        var categoryId = Guid.NewGuid();

        _repository.Setup(_ => _.GetEntityAsync<Category>(
                categoryId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category
            {
                Id = categoryId
            })
            .Verifiable();

        var first = await _context.GetCategoryById(categoryId, CancellationToken.None);

        Assert.IsNotNull(first);

        _repository
            .Verify(
                _ => _.GetEntityAsync<Category>(
                    categoryId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task GetCategoryById_DoesNotFetchFromRepositoryTwice()
    {
        var categoryId = Guid.NewGuid();

        _repository.Setup(_ => _.GetEntityAsync<Category>(
                categoryId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category
            {
                Id = categoryId
            })
            .Verifiable();

        await _context.GetCategoryById(categoryId, CancellationToken.None);


        _repository
            .Verify(
                _ => _.GetEntityAsync<Category>(
                    categoryId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
