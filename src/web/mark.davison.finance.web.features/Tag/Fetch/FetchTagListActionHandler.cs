namespace mark.davison.finance.web.features.Tag.Fetch;

public class FetchTagListActionHandler : IActionHandler<FetchTagListAction>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public FetchTagListActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task Handle(FetchTagListAction action, CancellationToken cancellationToken)
    {
        var response = await _repository.Get<TagListQueryResponse, TagListQueryRequest>(cancellationToken);

        _stateStore.SetState(new TagListState(response.Tags));
    }
}
