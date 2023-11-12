namespace mark.davison.finance.data.helpers.Seeders;

public class TransactionSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionSeeder(
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
    }

    public Task<CreateTransactionResponse> CreateTransaction(CreateTransactionRequest request)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTransactionRequest, CreateTransactionResponse>>();
        var currentUserContext = scope.ServiceProvider.GetRequiredService<ICurrentUserContext>();

        return handler.Handle(request, currentUserContext, CancellationToken.None);
    }
}
