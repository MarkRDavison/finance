namespace mark.davison.finance.bff.queries.test.Scenarios.AccountDashboardSummaryQuery;

[TestClass]
public class AccountDashboardSummaryQueryHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly AccountDashboardSummaryQueryHandler _handler;

    private Expression<Func<T, bool>>? ExtractExpression<T>(QueryParameters q)
        where T : class
    {
        var options = new JsonSerializerOptions().ConfigureRemoteLinq();
        var body = JsonSerializer.Deserialize<JsonObject>(q.CreateBody(), options);
        var expressionText = body!["where"]!.GetValue<string>();
        var deserialized = JsonSerializer
            .Deserialize<Remote.Linq.Expressions.Expression>(
                expressionText,
                options);
        return deserialized?.ToLinqExpression() as Expression<Func<T, bool>>;
    }

    public AccountDashboardSummaryQueryHandlerTests()
    {
        _httpRepository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns(string.Empty);
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new(_httpRepository.Object);

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>());

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
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

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>())
            .Verifiable();

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Transaction>())
            .Verifiable();

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<Account>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<Transaction>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Handle_AccountQueryWherePredicateWorks()
    {
        var accounts = new List<Account>
        {
            new Account { UserId = Guid.NewGuid() },
            new Account { UserId = Guid.NewGuid() },
            new Account { UserId = Guid.NewGuid() },
            new Account { UserId = Guid.NewGuid() },
            new Account { UserId = _currentUserContext.Object.CurrentUser.Id }
        };

        var request = new AccountDashboardSummaryQueryRequest
        {
            AccountTypeId = AccountConstants.Asset
        };

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                var expression = ExtractExpression<Account>(q);
                Assert.IsNotNull(expression);

                var results = accounts.Where(expression.Compile()).ToList();

                Assert.AreEqual(1, results.Count);

                return new List<Account>();
            })
            .Verifiable();

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<Account>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
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


        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);
        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                var expression = ExtractExpression<Transaction>(q);
                Assert.IsNotNull(expression);

                var results = transactions.Where(expression.Compile()).ToList();

                Assert.AreEqual(5, results.Count);

                return new List<Transaction>();
            })
            .Verifiable();

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<Transaction>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
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
            AccountTypeId = AccountConstants.Asset
        };

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Transaction>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        Assert.AreEqual(accounts.Count, response.AccountNames.Count);
        Assert.AreEqual(accounts.Count, response.TransactionData.Count);

        Assert.AreEqual(4, response.TransactionData[accounts[0].Id].Count);
        Assert.AreEqual(2, response.TransactionData[accounts[1].Id].Count);
        Assert.AreEqual(2, response.TransactionData[accounts[2].Id].Count);
    }
}
