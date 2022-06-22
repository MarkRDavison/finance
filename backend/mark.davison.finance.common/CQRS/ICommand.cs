namespace mark.davison.finance.common.CQRS;

public interface ICommand<TCommand, TResponse>
    where TCommand : class, new()
    where TResponse : class, new()
{
}

