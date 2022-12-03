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

    public Task<CreateTransactionCommandResponse> CreateTransaction(CreateTransactionCommandRequest request)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateTransactionCommandRequest, CreateTransactionCommandResponse>>();
        var currentUserContext = scope.ServiceProvider.GetRequiredService<ICurrentUserContext>();

        return handler.Handle(request, currentUserContext, CancellationToken.None);
    }
}
