namespace mark.davison.finance.common.client.CQRS;

public interface IActionDispatcher
{
    public Task Dispatch<TAction>(TAction action, CancellationToken cancellationToken)
        where TAction : class, IAction<TAction>;
    public Task Dispatch<TAction>(CancellationToken cancellationToken)
        where TAction : class, IAction<TAction>, new();
}
