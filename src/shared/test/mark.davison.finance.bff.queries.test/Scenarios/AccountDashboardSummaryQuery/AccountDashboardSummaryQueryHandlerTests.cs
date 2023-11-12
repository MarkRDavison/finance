using System.Linq.Expressions;

namespace mark.davison.finance.bff.queries.test.Scenarios.AccountDashboardSummaryQuery;

[TestClass]
public class AccountDashboardSummaryQueryHandlerTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly AccountDashboardSummaryQueryHandler _handler;

    public AccountDashboardSummaryQueryHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _repository.Setup(_ => _.BeginTransaction()).Returns(() => new TestAsyncDisposable());

        _handler = new(_repository.Object);

        _repository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>());

        _repository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Transaction>());
    }

    [TestMethod]
    public async Task Handle_InvokesRepository()
    {
        var request = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = AccountConstants.Asset
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>())
            .Verifiable();

        _repository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Transaction>())
            .Verifiable();

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Transaction>(
                    It.IsAny<Expression<Func<Transaction, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Handle_AccountQueryWherePredicateWorks()
    {
        var accounts = new List<Account>
        {
            new Account { AccountTypeId = AccountConstants.Asset, UserId = Guid.NewGuid() },
            new Account { AccountTypeId = AccountConstants.Asset, UserId = Guid.NewGuid() },
            new Account { AccountTypeId = AccountConstants.Asset, UserId = Guid.NewGuid() },
            new Account { AccountTypeId = AccountConstants.Asset, UserId = Guid.NewGuid() },
            new Account { AccountTypeId = AccountConstants.Asset, UserId = _currentUserContext.Object.CurrentUser.Id }
        };

        var request = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = AccountConstants.Asset
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Account, bool>> p, CancellationToken c) =>
            {
                var results = accounts.Where(p.Compile()).ToList();

                Assert.AreEqual(1, results.Count);

                return new List<Account>();
            })
            .Verifiable();

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Account>(
                    It.IsAny<Expression<Func<Account, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Handle_TransactionQueryWherePredicateWorks()
    {
        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid() },
            new Account { Id = Guid.NewGuid() },
            new Account { Id = Guid.NewGuid() },
            new Account { Id = Guid.NewGuid() },
            new Account { Id = Guid.NewGuid() }
        };
        var transactions = new List<Transaction>
        {
            new Transaction {  AccountId = accounts[0].Id, TransactionJournal = new() { TransactionTypeId = TransactionConstants.OpeningBalance } },
            new Transaction {  AccountId = accounts[1].Id, TransactionJournal = new() { TransactionTypeId = TransactionConstants.Deposit } },
            new Transaction {  AccountId = accounts[2].Id, TransactionJournal = new() { TransactionTypeId = TransactionConstants.LiabilityCredit } },
            new Transaction {  AccountId = accounts[3].Id, TransactionJournal = new() { TransactionTypeId = TransactionConstants.Transfer } },
            new Transaction {  AccountId = accounts[4].Id, TransactionJournal = new() { TransactionTypeId = TransactionConstants.Invalid } },
        };

        var request = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = AccountConstants.Asset
        };


        _repository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);
        _repository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Transaction, bool>> p, CancellationToken c) =>
            {
                var results = transactions.Where(p.Compile()).ToList();

                Assert.AreEqual(5, results.Count);

                return new List<Transaction>();
            })
            .Verifiable();

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Handle_RepositoryTransformationOfResultDataWorks()
    {
        var accounts = new List<Account>
        {
            new Account { Id = Guid.NewGuid() },
            new Account { Id = Guid.NewGuid() },
            new Account { Id = Guid.NewGuid() },
        };
        var transactions = new List<Transaction>
        {
            new Transaction { AccountId = accounts[0].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 1) } },
            new Transaction { AccountId = accounts[0].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 2) } },
            new Transaction { AccountId = accounts[0].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 3) } },
            new Transaction { AccountId = accounts[0].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 4) } },
            new Transaction { AccountId = accounts[1].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 1) } },
            new Transaction { AccountId = accounts[1].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 2) } },
            new Transaction { AccountId = accounts[2].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 1) } },
            new Transaction { AccountId = accounts[2].Id, TransactionJournal = new() { Date = new DateOnly(2022, 1, 2) } },
        };

        var request = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = AccountConstants.Asset,
            RangeStart = new DateOnly(2022, 1, 1),
            RangeEnd = new DateOnly(2022, 1, 31)
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        _repository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        Assert.AreEqual(accounts.Count, response.AccountNames.Count);
        Assert.AreEqual(accounts.Count, response.TransactionData.Count);

        var daysInMonth = request.RangeEnd.Day - request.RangeStart.Day + 1;

        Assert.AreEqual(daysInMonth, response.TransactionData[accounts[0].Id].Count);
        Assert.AreEqual(daysInMonth, response.TransactionData[accounts[1].Id].Count);
        Assert.AreEqual(daysInMonth, response.TransactionData[accounts[2].Id].Count);
    }
}
