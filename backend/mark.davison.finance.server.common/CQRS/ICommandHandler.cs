namespace mark.davison.finance.common.server.CQRS;

public interface ICommandHandler<in TCommand, TCommandResult>
    where TCommand : class, ICommand<TCommand, TCommandResult>, new()
    where TCommandResult : class, new()
{
    Task<TCommandResult> Handle(TCommand query, ICurrentUserContext currentUserContext, CancellationToken cancellation);
}