namespace mark.davison.finance.common.client.State;

public class StateStore : IStateStore
{
    private readonly IDictionary<string, IState> _state;
    private readonly IComponentSubscriptions _componentSubscriptions;

    public StateStore(IComponentSubscriptions componentSubscriptions)
    {
        _state = new Dictionary<string, IState>();
        _componentSubscriptions = componentSubscriptions;
    }

    public StateInstance<TState> GetState<TState>() where TState : class, IState, new()
    {
        if (!_state.ContainsKey(typeof(TState).Name))
        {
            var state = new TState();
            state.Initialise();
            _state[typeof(TState).Name] = state;
        }

        return new StateInstance<TState>(() => (TState)_state[typeof(TState).Name]);
    }

    public void Reset()
    {
        foreach (var (_, val) in _state)
        {
            val.Initialise();
            _componentSubscriptions.ReRenderSubscribers(val.GetType());
        }
    }

    public void SetState<TState>(TState state) where TState : class, IState, new()
    {
        _state[typeof(TState).Name] = state;
        _componentSubscriptions.ReRenderSubscribers(typeof(TState));
    }
}
