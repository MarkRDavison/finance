using System.Linq.Expressions;

namespace mark.davison.finance.bff.queries.test.Scenarios.AccountListQuery;

[TestClass]
public class AccountListQueryHandlerTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly AccountListQueryHandler _handler;

    public AccountListQueryHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new AccountListQueryHandler(_repository.Object);

        _repository
            .Setup(_ => _.GetEntitiesAsync<TransactionJournal>(
                It.IsAny<Expression<Func<TransactionJournal, bool>>>(),
                It.IsAny<Expression<Func<TransactionJournal, object>>[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TransactionJournal>());
    }


    [TestMethod]
    public async Task Handle_RetrievesAccountSummariesFromRepository()
    {
        var accounts = new List<Account> {
            new Account{ AccountType = new() },
            new Account{ AccountType = new() }
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<Expression<Func<Account, object>>[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts)
            .Verifiable();

        var request = new AccountListQueryRequest
        {
            ShowActive = true
        };
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(accounts.Count, response.Accounts.Count);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Account>(
                    It.IsAny<Expression<Func<Account, bool>>>(),
                    It.IsAny<Expression<Func<Account, object>>[]>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }


    [TestMethod]
    public async Task Handle_DoesNotRetrieveHiddenAccounts()
    {
        var accounts = new List<Account> {
            new Account{ Id = Account.OpeningBalance, AccountType = new() },
            new Account{ Id = Account.Reconciliation, AccountType = new() }
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<Account>(
                It.IsAny<Expression<Func<Account, bool>>>(),
                It.IsAny<Expression<Func<Account, object>>[]>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Account, bool>> p, Expression<Func<Account, object>>[] i, CancellationToken c) =>
            {
                return accounts.Where(p.Compile()).ToList();
            })
            .Verifiable();

        var request = new AccountListQueryRequest
        {
            ShowActive = true
        };
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(0, response.Accounts.Count);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Account>(
                    It.IsAny<Expression<Func<Account, bool>>>(),
                    It.IsAny<Expression<Func<Account, object>>[]>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    // TODO: Test to validate the opening balances
}

