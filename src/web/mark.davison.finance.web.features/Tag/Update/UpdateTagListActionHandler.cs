namespace mark.davison.finance.web.features.Tag.Update;

public class UpdateTagListActionHandler : IActionHandler<UpdateTagListAction>
{
    private readonly IStateStore _stateStore;

    public UpdateTagListActionHandler(
        IStateStore stateStore
    )
    {
        _stateStore = stateStore;
    }

    public Task Handle(UpdateTagListAction action, CancellationToken cancellationToken)
    {
        var existing = _stateStore.GetState<TagListState>().Instance.Tags;
        var updated = existing.ToDictionary(_ => _.Id, _ => _);

        foreach (var item in action.Items)
        {
            updated[item.Id] = item;
        }

        _stateStore.SetState(new TagListState(updated.Values));
        return Task.CompletedTask;
    }
}
