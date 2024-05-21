namespace mark.davison.finance.bff.queries.test.Scenarios.AccountListQuery;

[TestClass]
public class AccountListQueryHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly Mock<IUserApplicationContext> _userApplicationContext;
    private readonly AccountListQueryHandler _handler;
    private readonly CancellationToken _token;

    public AccountListQueryHandlerTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _currentUserContext = new(MockBehavior.Strict);
        _userApplicationContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new AccountListQueryHandler((IFinanceDbContext)_dbContext, _userApplicationContext.Object);

        _userApplicationContext
            .Setup(_ => _.LoadRequiredContext<FinanceUserApplicationContext>())
            .ReturnsAsync(new FinanceUserApplicationContext { });
    }

    // TODO: More tests around current balance and balance difference

    [TestMethod]
    public async Task Handle_RetrievesAccountSummariesFromRepository()
    {
        var accounts = new List<Account>
        {
            new Account{ Id = Guid.NewGuid(), UserId = _currentUserContext.Object.CurrentUser.Id, AccountType = new() { Id = Guid.NewGuid() } },
            new Account{ Id = Guid.NewGuid(), UserId = _currentUserContext.Object.CurrentUser.Id, AccountType = new() { Id = Guid.NewGuid() } }
        };

        await _dbContext.UpsertEntitiesAsync(accounts, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new AccountListQueryRequest
        {
            ShowActive = true
        };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.SuccessWithValue.Should().BeTrue();
        response.Value.Should().HaveCount(accounts.Count);
    }

    [TestMethod]
    public async Task Handle_DoesNotRetrieveHiddenAccounts()
    {
        var accounts = new List<Account> {
            new Account{ Id = AccountConstants.OpeningBalance, AccountType = new(){ Id = Guid.NewGuid() }},
            new Account{ Id = AccountConstants.Reconciliation, AccountType = new(){ Id = Guid.NewGuid() }}
        };

        var request = new AccountListQueryRequest
        {
            ShowActive = true
        };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.SuccessWithValue.Should().BeTrue();
        response.Value.Should().BeEmpty();
    }

    // TODO: Test to validate the opening balances
}

