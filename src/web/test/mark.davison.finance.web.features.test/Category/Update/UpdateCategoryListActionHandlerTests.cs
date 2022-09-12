namespace mark.davison.finance.web.features.test.Category.Update;

[TestClass]
public class UpdateCategoryListActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly UpdateCategoryListActionHandler _handler;

    public UpdateCategoryListActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _handler = new(_stateStore.Object);
    }


    [TestMethod]
    public async Task Handle_InvokesRepostoryAndStateStore()
    {
        var accountListItems = new List<CategoryListItemDto> {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        _stateStore
            .Setup(_ => _
                .GetState<CategoryListState>())
            .Returns(new StateInstance<CategoryListState>(() => new CategoryListState()));
        _stateStore
            .Setup(_ => _
                .SetState(
                    It.IsAny<CategoryListState>()))
            .Callback((CategoryListState newState) =>
            {
                Assert.AreEqual(accountListItems.Count, newState.Categories.Count());
            })
            .Verifiable();

        await _handler.Handle(new UpdateCategoryListAction(accountListItems), CancellationToken.None);

        _stateStore
            .Verify(_ => _
                .SetState(
                    It.IsAny<CategoryListState>()),
                Times.Once);
    }
}
