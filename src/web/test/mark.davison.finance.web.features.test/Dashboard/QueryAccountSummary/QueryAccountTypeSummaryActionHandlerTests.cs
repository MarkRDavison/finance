namespace mark.davison.finance.web.features.test.Dashboard.QueryAccountSummary;

[TestClass]
public class QueryAccountTypeSummaryActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly Mock<IAppContextService> _appContextService;
    private readonly QueryAccountTypeSummaryActionHandler _handler;

    public QueryAccountTypeSummaryActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _repository = new(MockBehavior.Strict);
        _appContextService = new(MockBehavior.Strict);
        _handler = new(_repository.Object, _stateStore.Object, _appContextService.Object);
    }

    [TestMethod]
    public async Task Handle_InvokesRepostoryAndStateStore()
    {
        var account1Id = Guid.NewGuid();
        var account2Id = Guid.NewGuid();
        var account3Id = Guid.NewGuid();
        var account4Id = Guid.NewGuid();

        var existingAccountNames = new Dictionary<Guid, string>
        {
            { account1Id, "Account1" },
            { account2Id, "Account2" },
            { account3Id, "Account3" }
        };
        var existingTransactionData = new Dictionary<Guid, List<AccountDashboardTransactionData>>
        {
            { account1Id, new() { new(), new() } },
            { account2Id, new() { new(), new() } },
            { account3Id, new() { new(), new() } }
        };
        var newAccountNames = new Dictionary<Guid, string>
        {
            { account3Id, "Account3NewName" },
            { account4Id, "Account4" }
        };
        var newTransactionData = new Dictionary<Guid, List<AccountDashboardTransactionData>>
        {
            { account3Id, new() { new(), new() } },
            { account4Id, new() { new(), new() } }
        };

        var request = new QueryAccountSummaryActionRequest
        {
            AccountTypeId = AccountConstants.Asset
        };

        var rangeStart = DateOnly.FromDateTime(DateTime.Now);
        var rangeEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(31));

        _appContextService.Setup(_ => _.RangeStart).Returns(rangeStart);
        _appContextService.Setup(_ => _.RangeEnd).Returns(rangeEnd);

        _repository
            .Setup(_ => _.Get<AccountDashboardSummaryQueryResponse, AccountDashboardSummaryQueryRequest>(
                It.IsAny<AccountDashboardSummaryQueryRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback((AccountDashboardSummaryQueryRequest r, CancellationToken c) =>
            {
                Assert.AreEqual(request.AccountTypeId, r.AccountTypeId);
                Assert.AreEqual(rangeStart, r.RangeStart);
                Assert.AreEqual(rangeEnd, r.RangeEnd);
            })
            .ReturnsAsync(new AccountDashboardSummaryQueryResponse()
            {
                Success = true,
                AccountNames = newAccountNames,
                TransactionData = newTransactionData
            })
            .Verifiable();

        _stateStore
            .Setup(_ => _
                .GetState<DashboardState>())
            .Returns(new StateInstance<DashboardState>(() => new DashboardState(existingAccountNames, existingTransactionData)));
        _stateStore
            .Setup(_ => _
                .SetState<DashboardState>(
                    It.IsAny<DashboardState>()))
            .Callback((DashboardState newState) =>
            {
                Assert.AreEqual(4, newState.AccountNames.Count);
                Assert.AreEqual("Account3NewName", newState.AccountNames[account3Id]);
                Assert.AreEqual(4, newState.TransactionData.Count);
            })
            .Verifiable();

        await _handler.Handle(request, CancellationToken.None);

        _repository
            .Verify(_ =>
                _.Get<AccountDashboardSummaryQueryResponse, AccountDashboardSummaryQueryRequest>(
                    It.IsAny<AccountDashboardSummaryQueryRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _stateStore
            .Verify(
                _ => _
                    .SetState<DashboardState>(
                        It.IsAny<DashboardState>()),
                Times.Once);
    }

}
