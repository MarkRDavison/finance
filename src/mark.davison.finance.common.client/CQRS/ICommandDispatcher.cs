namespace mark.davison.finance.common.client.CQRS;

public interface ICommandDispatcher
{
    Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TCommand, TCommandResult>
        where TCommandResult : class, new();
    Task<TCommandResult> Dispatch<TCommand, TCommandResult>(CancellationToken cancellationToken)
        where TCommand : class, ICommand<TCommand, TCommandResult>, new()
        where TCommandResult : class, new();
}
