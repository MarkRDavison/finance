namespace mark.davison.finance.common.client.CQRS;

public interface IActionHandler<in TAction>
    where TAction : class, IAction<TAction>
{
    Task Handle(TAction action, CancellationToken cancellationToken);
}
