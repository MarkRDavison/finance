namespace mark.davison.finance.common.client.CQRS;

public class CommandDispatcher : ICommandDispatcher
{
    public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, ICommandHandler<TCommand, TCommandResult> handler, CancellationToken cancellation)
        where TCommand : class, ICommand<TCommand, TCommandResult>, new()
        where TCommandResult : class, new()
    {

        return handler.Handle(command, cancellation);
    }
}
