using System.Linq.Expressions;

namespace mark.davison.finance.bff.queries.test.Scenarios.TransactionByAccountQuery;

[TestClass]
public class TransactionByAccountQueryHandlerTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TransactionByAccountQueryHandler _handler;

    public TransactionByAccountQueryHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new TransactionByAccountQueryHandler(_repository.Object);
    }

    [TestMethod]
    public async Task Handle_InvokesRepository()
    {
        Guid accountId = Guid.NewGuid();
        _repository
            .Setup(_ => _.
                GetEntitiesAsync<TransactionJournal>(
                    It.IsAny<Expression<Func<TransactionJournal, bool>>>(),
                    It.IsAny<Expression<Func<TransactionJournal, object>>[]>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TransactionJournal>())
            .Verifiable();

        await _handler.Handle(new TransactionByAccountQueryRequest { AccountId = accountId }, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(_ => _.
                GetEntitiesAsync<Transaction>(
                    It.IsAny<Expression<Func<Transaction, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [TestMethod]
    public async Task Handle_ReturnsRepositoryReturnedTransactions()
    {
        var transactionJournals = new List<TransactionJournal> {
                new TransactionJournal {  },
                new TransactionJournal {  }
        };
        _repository
            .Setup(_ => _.
                GetEntitiesAsync<TransactionJournal>(
                    It.IsAny<Expression<Func<TransactionJournal, bool>>>(),
                    It.IsAny<Expression<Func<TransactionJournal, object>>[]>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionJournals);

        var response = await _handler.Handle(new TransactionByAccountQueryRequest { }, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(transactionJournals.Count, response.Transactions.Count);
    }
}
