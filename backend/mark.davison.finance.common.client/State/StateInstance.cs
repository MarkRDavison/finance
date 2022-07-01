namespace mark.davison.finance.common.client.State;

public class StateInstance<TState> where TState : IState
{
    public StateInstance(Func<TState> fetcher)
    {
        _fetcher = fetcher;
    }

    public TState Instance => _fetcher();

    private Func<TState> _fetcher;
}
