namespace mark.davison.finance.shared.queries.test.Scenarios.AccountDashboardSummaryQuery;

[TestClass]
public class AccountDashboardSummaryQueryHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly Mock<IFinanceUserContext> _financeUserContext;
    private readonly AccountDashboardSummaryQueryHandler _handler;
    private readonly CancellationToken _token;

    public AccountDashboardSummaryQueryHandlerTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _financeUserContext = new(MockBehavior.Strict);
        _financeUserContext.Setup(_ => _.RangeStart).Returns(DateOnly.MinValue);
        _financeUserContext.Setup(_ => _.RangeEnd).Returns(DateOnly.MaxValue);
        _financeUserContext.Setup(_ => _.LoadAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new((IFinanceDbContext)_dbContext, _financeUserContext.Object);
    }

    [TestMethod]
    public async Task Handle_TransactionQueryWherePredicateWorks()
    {
        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id },
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id },
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id },
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id },
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id }
        };

        var transactions = new List<Transaction>
        {
            new Transaction {  Id = Guid.NewGuid(), AccountId = accounts[0].Id, TransactionJournal = new() { Id = Guid.NewGuid(), TransactionTypeId = TransactionTypeConstants.OpeningBalance } },
            new Transaction {  Id = Guid.NewGuid(), AccountId = accounts[1].Id, TransactionJournal = new() { Id = Guid.NewGuid(), TransactionTypeId = TransactionTypeConstants.Deposit } },
            new Transaction {  Id = Guid.NewGuid(), AccountId = accounts[2].Id, TransactionJournal = new() { Id = Guid.NewGuid(), TransactionTypeId = TransactionTypeConstants.LiabilityCredit } },
            new Transaction {  Id = Guid.NewGuid(), AccountId = accounts[3].Id, TransactionJournal = new() { Id = Guid.NewGuid(), TransactionTypeId = TransactionTypeConstants.Transfer } },
            new Transaction {  Id = Guid.NewGuid(), AccountId = accounts[4].Id, TransactionJournal = new() { Id = Guid.NewGuid(), TransactionTypeId = TransactionTypeConstants.Invalid } },
        };

        await _dbContext.UpsertEntitiesAsync(transactions, _token);
        await _dbContext.UpsertEntitiesAsync(accounts, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = AccountTypeConstants.Asset
        };


        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.SuccessWithValue.Should().BeTrue();

        response.Value!.TransactionData.Should().HaveCount(5);

        foreach (var (_, transactionData) in response.Value.TransactionData)
        {
            transactionData.Should().HaveCount(1);
        }
    }

    [TestMethod]
    public async Task Handle_RepositoryTransformationOfResultDataWorks()
    {
        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id },
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id },
            new Account { Id = Guid.NewGuid(), AccountTypeId = AccountTypeConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id },
        };
        var transactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[0].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 1) } },
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[0].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 2) } },
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[0].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 3) } },
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[0].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 4) } },
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[1].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 1) } },
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[1].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 2) } },
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[2].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 1) } },
            new Transaction { Id = Guid.NewGuid(), AccountId = accounts[2].Id, TransactionJournal = new() { Id = Guid.NewGuid(), Date = new DateOnly(2022, 1, 2) } },
        };

        await _dbContext.UpsertEntitiesAsync(transactions, _token);
        await _dbContext.UpsertEntitiesAsync(accounts, _token);
        await _dbContext.SaveChangesAsync(_token);

        var rangeStart = new DateOnly(2022, 1, 1);
        var rangeEnd = new DateOnly(2022, 1, 31);

        var request = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = AccountTypeConstants.Asset
        };
        _financeUserContext.Setup(_ => _.RangeStart).Returns(rangeStart);
        _financeUserContext.Setup(_ => _.RangeEnd).Returns(rangeEnd);

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.SuccessWithValue.Should().BeTrue();

        Assert.AreEqual(accounts.Count, response.Value.TransactionData.Count);

        var daysInMonth = rangeStart.Day - rangeEnd.Day + 1;

        Assert.AreEqual(daysInMonth, response.Value.TransactionData[accounts[0].Id].Count);
        Assert.AreEqual(daysInMonth, response.Value.TransactionData[accounts[1].Id].Count);
        Assert.AreEqual(daysInMonth, response.Value.TransactionData[accounts[2].Id].Count);
    }
}
