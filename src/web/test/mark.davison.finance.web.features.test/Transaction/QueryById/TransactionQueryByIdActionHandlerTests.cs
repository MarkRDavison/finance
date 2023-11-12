namespace mark.davison.finance.web.features.test.Transaction.QueryById;

[TestClass]
public class TransactionQueryByIdActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly TransactionQueryByIdActionHandler _handler;

    public TransactionQueryByIdActionHandlerTests()
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
                .Get<TransactionByIdQueryResponse, TransactionByIdQueryRequest>(
                    It.IsAny<TransactionByIdQueryRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((TransactionByIdQueryRequest req, CancellationToken cancellationToken) => new TransactionByIdQueryResponse()
            {
                Transactions = transactions
            })
            .Verifiable();

        await _handler.Handle(new TransactionQueryByIdAction
        {
            TransactionGroupId = Guid.NewGuid()
        }, CancellationToken.None);

        _repository
            .Verify(_ => _
                .Get<TransactionByIdQueryResponse, TransactionByIdQueryRequest>(
                    It.IsAny<TransactionByIdQueryRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _stateStore
            .Verify(_ => _
                .SetState<TransactionState>(
                    It.IsAny<TransactionState>()),
                Times.Once);
    }
}