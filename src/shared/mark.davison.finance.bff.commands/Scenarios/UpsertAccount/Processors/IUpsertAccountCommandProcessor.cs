namespace mark.davison.finance.bff.commands.Scenarios.UpsertAccount.Processors;

public interface IUpsertAccountCommandProcessor
{
    Task<UpsertAccountResponse> Process(
        UpsertAccountRequest request,
        UpsertAccountResponse response,
        ICurrentUserContext currentUserContext,
        IHttpRepository httpRepository,
        CancellationToken cancellationToken);
}
