namespace mark.davison.finance.common.client.CQRS;

public interface ICommandHandler<in TCommand, TCommandResult>
    where TCommand : class, ICommand<TCommand, TCommandResult>, new()
    where TCommandResult : class, new()
{
    Task<TCommandResult> Handle(TCommand command, CancellationToken cancellation);
}