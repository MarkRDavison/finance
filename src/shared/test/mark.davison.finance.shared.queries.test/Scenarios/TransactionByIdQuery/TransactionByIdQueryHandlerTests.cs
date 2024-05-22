namespace mark.davison.finance.shared.queries.test.Scenarios.TransactionByIdQuery;

[TestClass]
public class TransactionByIdQueryHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TransactionByIdQueryHandler _handler;
    private readonly CancellationToken _token;

    public TransactionByIdQueryHandlerTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _currentUserContext = new(MockBehavior.Strict);
        _handler = new((IFinanceDbContext)_dbContext);

        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });
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

        await _dbContext.UpsertEntitiesAsync(transactionJournals, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new TransactionByIdQueryRequest { TransactionGroupId = Guid.NewGuid() };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();
    }
}
