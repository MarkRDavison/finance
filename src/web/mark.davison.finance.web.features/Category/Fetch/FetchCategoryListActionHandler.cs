namespace mark.davison.finance.web.features.Category.Fetch;

public class FetchCategoryListActionHandler : IActionHandler<FetchCategoryListAction>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public FetchCategoryListActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task Handle(FetchCategoryListAction action, CancellationToken cancellationToken)
    {
        var response = await _repository.Get<CategoryListQueryResponse, CategoryListQueryRequest>(new CategoryListQueryRequest(), cancellationToken);

        _stateStore.SetState(new CategoryListState(response.Categories));
    }
}
