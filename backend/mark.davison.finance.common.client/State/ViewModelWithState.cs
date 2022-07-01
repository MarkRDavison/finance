namespace mark.davison.finance.common.client.State;

public class ViewModelWithState : IViewModelWithState, IDisposable
{
    private readonly IStateStore _stateStore;
    private readonly IComponentSubscriptions _componentSubscriptions;
    private readonly List<IComponentWithState> _relatedComponents = new();

    public ViewModelWithState(
        IStateStore stateStore,
        IComponentSubscriptions componentSubscriptions,
        ICQRSDispatcher dispatcher)
    {
        _stateStore = stateStore;
        _componentSubscriptions = componentSubscriptions;
        Dispatcher = dispatcher;
    }


    protected ICQRSDispatcher Dispatcher { get; init; }

    public IEnumerable<IComponentWithState> RelatedComponents => _relatedComponents;

    public void AddRelatedComponent(IComponentWithState componentWithState)
    {
        _relatedComponents.Add(componentWithState);
    }

    public void Dispose()
    {
        foreach (var component in _relatedComponents)
        {
            _relatedComponents.ForEach(_componentSubscriptions.Remove);
        }
        _relatedComponents.Clear();
    }

    public StateInstance<TState> GetState<TState>() where TState : class, IState, new()
    {
        _relatedComponents.ForEach(_componentSubscriptions.Add<TState>);
        return _stateStore.GetState<TState>();
    }

}
