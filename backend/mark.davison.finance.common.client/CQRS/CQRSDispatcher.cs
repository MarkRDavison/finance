namespace mark.davison.finance.common.client.CQRS;

public interface ICQRSDispatcher : ICommandDispatcher, IActionDispatcher
{

}

public class CQRSDispatcher : ICQRSDispatcher
{

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CQRSDispatcher(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand<TCommand, TCommandResult>
        where TCommandResult : class, new()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
        return handler.Handle(command, cancellationToken);
    }

    public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(CancellationToken cancellationToken)
        where TCommand : class, ICommand<TCommand, TCommandResult>, new()
        where TCommandResult : class, new()
    {
        return Dispatch<TCommand, TCommandResult>(new TCommand(), cancellationToken);
    }

    public Task Dispatch<TAction>(TAction action, CancellationToken cancellationToken)
        where TAction : class, IAction<TAction>
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IActionHandler<TAction>>();
        return handler.Handle(action, cancellationToken);
    }

    public Task Dispatch<TAction>(CancellationToken cancellationToken)
        where TAction : class, IAction<TAction>, new()
    {
        return Dispatch<TAction>(new TAction(), cancellationToken);
    }
}
