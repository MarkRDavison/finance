namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Processors;

public interface ICreateTransactionCommandProcessor
{
    Task<CreateTransactionCommandResponse> Process(CreateTransactionCommandRequest request, CreateTransactionCommandResponse response, ICurrentUserContext currentUserContext, IHttpRepository httpRepository, CancellationToken cancellationToken);
}
