namespace mark.davison.finance.bff.commands.test.Scenarios.CreateTransaction.Common.Validators;

[TestClass]
public class CreateTransctionValidationContextTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateTransctionValidationContext _context;
    private readonly CancellationToken _token;

    public CreateTransctionValidationContextTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _currentUserContext = new(MockBehavior.Strict);
        _context = new((IFinanceDbContext)_dbContext, _currentUserContext.Object);

        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User());

    }

    [TestMethod]
    public async Task GetAccountById_FetchesFromRepository()
    {
        var accountId = Guid.NewGuid();

        await _dbContext.UpsertEntityAsync(new Account { Id = accountId }, _token);
        await _dbContext.SaveChangesAsync(_token);

        var first = await _context.GetAccountById(accountId, CancellationToken.None);

        Assert.IsNotNull(first);
    }

    [TestMethod]
    public async Task GetCategoryById_FetchesFromRepository()
    {
        var categoryId = Guid.NewGuid();

        await _dbContext.UpsertEntityAsync(new Category { Id = categoryId }, _token);
        await _dbContext.SaveChangesAsync(_token);

        var first = await _context.GetCategoryById(categoryId, CancellationToken.None);

        Assert.IsNotNull(first);
    }
}
