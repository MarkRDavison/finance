namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Common.Validators;

[TestClass]
public class CreateTransctionValidationContextTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateTransctionValidationContext _context;

    public CreateTransctionValidationContextTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _context = new(_httpRepository.Object, _currentUserContext.Object);

        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User());
    }

    [TestMethod]
    public async Task GetAccountById_FetchesFromHttpRepository()
    {
        var accountId = Guid.NewGuid();

        _httpRepository
            .Setup(_ => _.GetEntityAsync<Account>(accountId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Account
            {
                Id = accountId
            })
            .Verifiable();

        var first = await _context.GetAccountById(accountId, CancellationToken.None);
        var second = await _context.GetAccountById(accountId, CancellationToken.None);

        Assert.IsNotNull(first);
        Assert.IsNotNull(second);
        Assert.AreEqual(first.Id, second.Id);

        _httpRepository
            .Verify(
                _ => _.GetEntityAsync<Account>(accountId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task GetAccountById_DoesNotFetchFromRepositoryTwice()
    {
        var accountId = Guid.NewGuid();

        _httpRepository
            .Setup(_ => _.GetEntityAsync<Account>(accountId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Account())
            .Verifiable();

        await _context.GetAccountById(accountId, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.GetEntityAsync<Account>(accountId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task GetCategoryById_FetchesFromHttpRepository()
    {
        var categoryId = Guid.NewGuid();

        _httpRepository
            .Setup(_ => _.GetEntityAsync<Category>(categoryId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category
            {
                Id = categoryId
            })
            .Verifiable();

        var first = await _context.GetCategoryById(categoryId, CancellationToken.None);
        var second = await _context.GetCategoryById(categoryId, CancellationToken.None);

        Assert.IsNotNull(first);
        Assert.IsNotNull(second);
        Assert.AreEqual(first.Id, second.Id);

        _httpRepository
            .Verify(
                _ => _.GetEntityAsync<Category>(categoryId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task GetCategoryById_DoesNotFetchFromRepositoryTwice()
    {
        var categoryId = Guid.NewGuid();

        _httpRepository
            .Setup(_ => _.GetEntityAsync<Category>(categoryId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category())
            .Verifiable();

        await _context.GetCategoryById(categoryId, CancellationToken.None);

        _httpRepository
            .Verify(
                _ => _.GetEntityAsync<Category>(categoryId, It.IsAny<HeaderParameters>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
