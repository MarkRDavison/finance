namespace mark.davison.finance.common.client.Repository;

public interface IClientHttpRepository
{
    // TODO: CancellationToken?
    Task<TResponse> Get<TResponse, TRequest>(TRequest request)
        where TRequest : class, ICommand<TRequest, TResponse>, new()
        where TResponse : class, new();
    Task<TResponse> Post<TResponse, TRequest>(TRequest request)
        where TRequest : class, ICommand<TRequest, TResponse>, new()
        where TResponse : class, new();
}
