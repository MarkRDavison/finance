namespace mark.davison.finance.common.client.State;

public interface IComponentSubscriptions
{
    void Add<TState>(IComponentWithState component) where TState : class, IState, new();
    void Remove(IComponentWithState component);

    void ReRenderSubscribers<TState>() where TState : class, IState, new();
    void ReRenderSubscribers(Type stateType);
}
