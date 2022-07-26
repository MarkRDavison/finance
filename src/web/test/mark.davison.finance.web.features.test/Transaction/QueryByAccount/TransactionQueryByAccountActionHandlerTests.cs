namespace mark.davison.finance.web.features.test.Transaction.QueryByAccount;

[TestClass]
public class TransactionQueryByAccountActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly TransactionQueryByAccountActionHandler _handler;

    public TransactionQueryByAccountActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _repository = new(MockBehavior.Strict);
        _handler = new(_repository.Object, _stateStore.Object);
    }


    [TestMethod]
    public async Task Handle_InvokesRepostoryAndStateStore()
    {
        var transactions = new List<TransactionDto> {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        _stateStore
            .Setup(_ => _
                .GetState<TransactionState>())
            .Returns(new StateInstance<TransactionState>(() => new TransactionState(transactions)));
        _stateStore
            .Setup(_ => _
                .SetState<TransactionState>(
                    It.IsAny<TransactionState>()))
            .Callback((TransactionState newState) =>
            {
                Assert.AreEqual(transactions.Count, newState.Transactions.Count());
            })
            .Verifiable();

        _repository
            .Setup(_ => _
                .Get<TransactionByAccountQueryResponse, TransactionByAccountQueryRequest>(
                    It.IsAny<TransactionByAccountQueryRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransactionByAccountQueryRequest req) => new TransactionByAccountQueryResponse()
            {
                Transactions = transactions
            })
            .Verifiable();

        await _handler.Handle(new TransactionQueryByAccountAction
        {
            AccountId = Guid.NewGuid()
        }, CancellationToken.None);

        _repository
            .Verify(_ => _
                .Get<TransactionByAccountQueryResponse, TransactionByAccountQueryRequest>(
                    It.IsAny<TransactionByAccountQueryRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _stateStore
            .Verify(_ => _
                .SetState<TransactionState>(
                    It.IsAny<TransactionState>()),
                Times.Once);
    }
}
