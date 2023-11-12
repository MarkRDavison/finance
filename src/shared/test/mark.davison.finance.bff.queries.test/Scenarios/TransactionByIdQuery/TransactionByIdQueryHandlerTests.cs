using mark.davison.finance.bff.queries.Scenarios.TransactionByIdQuery;
using mark.davison.finance.models.dtos.Queries.TransactionByIdQuery;

namespace mark.davison.finance.bff.queries.test.Scenarios.TransactionByIdQuery;

[TestClass]
public class TransactionByIdQueryHandlerTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TransactionByIdQueryHandler _handler;

    public TransactionByIdQueryHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _handler = new(_repository.Object);

        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _repository.Setup(_ => _.BeginTransaction()).Returns(() => new TestAsyncDisposable());

    }

    [TestMethod]
    public async Task Handler_RetrievesTransactionJournals()
    {
        var transactionJournals = new List<TransactionJournal>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TransactionGroup = new()
                {
                Id = Guid.NewGuid(),
                },
                Transactions = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                    }
                }
            }
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<TransactionJournal>(
                It.IsAny<Expression<Func<TransactionJournal, bool>>>(),
                It.IsAny<Expression<Func<TransactionJournal, object>>[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionJournals)
            .Verifiable();

        var request = new TransactionByIdQueryRequest { TransactionGroupId = Guid.NewGuid() };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<TransactionJournal>(
                    It.IsAny<Expression<Func<TransactionJournal, bool>>>(),
                    It.IsAny<Expression<Func<TransactionJournal, object>>[]>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        Assert.IsTrue(response.Success);
    }
}
