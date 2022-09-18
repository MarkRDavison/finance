namespace mark.davison.finance.bff.queries.test.Scenarios.AccountListQuery;

[TestClass]
public class AccountListQueryHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly AccountListQueryHandler _handler;

    public AccountListQueryHandlerTests()
    {
        _httpRepository = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new AccountListQueryHandler(_httpRepository.Object);
    }


    [TestMethod]
    public async Task Handle_RetrievesAccountSummariesFromRepository()
    {
        var accountSummaries = new List<AccountSummary> {
            new AccountSummary{ },
            new AccountSummary{ }
        };

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<AccountSummary>(
                "account/summary",
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountSummaries)
            .Verifiable();

        var request = new AccountListQueryRequest
        {
            ShowActive = true
        };
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(accountSummaries.Count, response.Accounts.Count);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<AccountSummary>(
                    "account/summary", // TODO: magic string
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }


    [TestMethod]
    public async Task Handle_DoesNotRetrieveHiddenAccounts()
    {
        var accountSummaries = new List<AccountSummary> {
            new AccountSummary{ Id = Account.OpeningBalance },
            new AccountSummary{ Id = Account.Reconciliation }
        };

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<AccountSummary>(
                "account/summary", // TODO: magic string
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountSummaries)
            .Verifiable();

        var request = new AccountListQueryRequest
        {
            ShowActive = true
        };
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(0, response.Accounts.Count);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<AccountSummary>(
                    "account/summary", // TODO: magic string
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

