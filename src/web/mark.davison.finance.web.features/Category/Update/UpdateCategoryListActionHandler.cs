namespace mark.davison.finance.web.features.Category.Update;

public class UpdateCategoryListActionHandler : IActionHandler<UpdateCategoryListAction>
{
    private readonly IStateStore _stateStore;

    public UpdateCategoryListActionHandler(
        IStateStore stateStore
    )
    {
        _stateStore = stateStore;
    }

    public Task Handle(UpdateCategoryListAction action, CancellationToken cancellationToken)
    {
        var existing = _stateStore.GetState<CategoryListState>().Instance.Categories;
        var updated = existing.ToDictionary(_ => _.Id, _ => _);

        foreach (var item in action.Items)
        {
            updated[item.Id] = item;
        }

        _stateStore.SetState(new CategoryListState(updated.Values));
        return Task.CompletedTask;
    }
}
