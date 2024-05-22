namespace mark.davison.finance.shared.queries.test.Scenarios.TransactionByAccountQuery;

[TestClass]
public class TransactionByAccountQueryHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TransactionByAccountQueryHandler _handler;
    private readonly CancellationToken _token;

    public TransactionByAccountQueryHandlerTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new TransactionByAccountQueryHandler((IFinanceDbContext)_dbContext);
    }

    [TestMethod]
    public async Task Handle_ReturnsTransactions()
    {
        var request = new TransactionByAccountQueryRequest
        {
            AccountId = Guid.NewGuid()
        };
        var transactionJournals = new List<TransactionJournal>
        {
            new TransactionJournal {
                Id = Guid.NewGuid(),
                Transactions = new() {
                    new Transaction() { Id = Guid.NewGuid(), AccountId = request.AccountId },
                    new Transaction() { Id = Guid.NewGuid(), AccountId = request.AccountId }
                },
                TransactionGroup = new() { Id = Guid.NewGuid() }
            },
            new TransactionJournal {
                Id = Guid.NewGuid(),
                Transactions = new() {
                    new Transaction() { Id = Guid.NewGuid(), AccountId = request.AccountId },
                    new Transaction() { Id = Guid.NewGuid(), AccountId = request.AccountId }
                },
                TransactionGroup = new() { Id = Guid.NewGuid() }
            }
        };

        await _dbContext.UpsertEntitiesAsync(transactionJournals, _token);
        await _dbContext.SaveChangesAsync(_token);

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.SuccessWithValue.Should().BeTrue();
        response.Value.Should().HaveCount(transactionJournals.SelectMany(_ => _.Transactions).Count());
    }
}
