namespace mark.davison.finance.web.features.test.Category.List;

[TestClass]
public class FetchCategoryListActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly FetchCategoryListActionHandler _handler;

    public FetchCategoryListActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _repository = new(MockBehavior.Strict);
        _handler = new(_repository.Object, _stateStore.Object);
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
                .SetState<CategoryListState>(
                    It.IsAny<CategoryListState>()))
            .Callback((CategoryListState newState) =>
            {
                Assert.AreEqual(accountListItems.Count, newState.Categories.Count());
            })
            .Verifiable();

        _repository
            .Setup(_ => _
                .Get<CategoryListQueryResponse, CategoryListQueryRequest>(
                    It.IsAny<CategoryListQueryRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryListQueryRequest req, CancellationToken cancellationToken) => new CategoryListQueryResponse()
            {
                Categories = accountListItems
            })
            .Verifiable();

        await _handler.Handle(new FetchCategoryListAction(), CancellationToken.None);

        _repository
            .Verify(_ => _
                .Get<CategoryListQueryResponse, CategoryListQueryRequest>(
                    It.IsAny<CategoryListQueryRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _stateStore
            .Verify(_ => _
                .SetState<CategoryListState>(
                    It.IsAny<CategoryListState>()),
                Times.Once);
    }
}