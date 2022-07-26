namespace mark.davison.finance.web.features.test.Lookup;

[TestClass]
public class UpdateLookupsActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly UpdateLookupsActionHandler _handler;

    public UpdateLookupsActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _repository = new(MockBehavior.Strict);
        _handler = new(_repository.Object, _stateStore.Object);
    }

    [TestMethod]
    public async Task Handle_InvokesRepositoryAndStore()
    {
        _repository
            .Setup(_ => _
                .Get<StartupQueryResponse, StartupQueryRequest>(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken cancellationToken) => new StartupQueryResponse())
            .Verifiable();

        _stateStore
            .Setup(_ => _
                .SetState<LookupState>(
                    It.IsAny<LookupState>()))
            .Verifiable();

        await _handler.Handle(new UpdateLookupsAction(), CancellationToken.None);

        _stateStore
            .Verify(_ => _
                .SetState<LookupState>(
                    It.IsAny<LookupState>()),
                Times.Once);

        _repository
            .Verify(_ => _
                .Get<StartupQueryResponse, StartupQueryRequest>(
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
