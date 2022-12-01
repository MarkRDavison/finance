namespace mark.davison.finance.bff.commands.Scenarios.UpsertAccount.Processors;

public interface IUpsertAccountCommandProcessor
{
    Task<UpsertAccountCommandResponse> Process(
        UpsertAccountCommandRequest request,
        UpsertAccountCommandResponse response,
        ICurrentUserContext currentUserContext,
        IHttpRepository httpRepository,
        CancellationToken cancellationToken);
}
