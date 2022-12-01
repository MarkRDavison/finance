namespace mark.davison.finance.web.features.test.Tag.Update;

[TestClass]
public class UpdateTagListActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly UpdateTagListActionHandler _handler;

    public UpdateTagListActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _handler = new(_stateStore.Object);
    }


    [TestMethod]
    public async Task Handle_InvokesRepostoryAndStateStore()
    {
        var items = new List<TagDto> {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        _stateStore
            .Setup(_ => _
                .GetState<TagListState>())
            .Returns(new StateInstance<TagListState>(() => new TagListState()));
        _stateStore
            .Setup(_ => _
                .SetState(
                    It.IsAny<TagListState>()))
            .Callback((TagListState newState) =>
            {
                Assert.AreEqual(items.Count, newState.Tags.Count());
            })
            .Verifiable();

        await _handler.Handle(new UpdateTagListAction(items), CancellationToken.None);

        _stateStore
            .Verify(_ => _
                .SetState(
                    It.IsAny<TagListState>()),
                Times.Once);
    }
}
