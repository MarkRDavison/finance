namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Processors;

public interface ICreateTransactionCommandProcessor
{
    Task<CreateTransactionResponse> Process(CreateTransactionRequest request, CreateTransactionResponse response, ICurrentUserContext currentUserContext, IHttpRepository httpRepository, CancellationToken cancellation);
}
