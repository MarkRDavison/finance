namespace mark.davison.finance.bff.queries.test.Scenarios.TransactionByAccountQuery;

[TestClass]
public class TransactionByAccountQueryHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TransactionByAccountQueryHandler _handler;

    public TransactionByAccountQueryHandlerTests()
    {
        _httpRepositoryMock = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new TransactionByAccountQueryHandler(_httpRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_InvokesHttpRepository()
    {
        Guid accountId = Guid.NewGuid();
        _httpRepositoryMock
            .Setup(_ => _.
                GetEntitiesAsync<Transaction>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                Assert.IsTrue(q.ContainsKey(nameof(Transaction.AccountId)));
                Assert.AreEqual(accountId.ToString(), q[nameof(Transaction.AccountId)]);
                return new List<Transaction>();
            })
            .Verifiable();

        await _handler.Handle(new TransactionByAccountQueryRequest { AccountId = accountId }, _currentUserContext.Object, CancellationToken.None);

        _httpRepositoryMock
            .Verify(_ => _.
                GetEntitiesAsync<Transaction>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Handle_ReturnsHttpRepositoryReturnedTransactions()
    {
        var transactions = new List<Transaction> {
                new Transaction {  },
                new Transaction {  }
        };
        _httpRepositoryMock
            .Setup(_ => _.
                GetEntitiesAsync<Transaction>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        var response = await _handler.Handle(new TransactionByAccountQueryRequest { }, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(transactions.Count, response.Transactions.Count);
    }
}
