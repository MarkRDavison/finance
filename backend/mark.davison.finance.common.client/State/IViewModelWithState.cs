namespace mark.davison.finance.common.client.State;

public interface IViewModelWithState
{
    IEnumerable<IComponentWithState> RelatedComponents { get; }
    StateInstance<TState> GetState<TState>() where TState : class, IState, new();
}
