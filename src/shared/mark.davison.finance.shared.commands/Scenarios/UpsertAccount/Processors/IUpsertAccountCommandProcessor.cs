namespace mark.davison.finance.shared.commands.Scenarios.UpsertAccount.Processors;

public interface IUpsertAccountCommandProcessor
{
    Task<UpsertAccountCommandResponse> Process(
        UpsertAccountCommandRequest request,
        UpsertAccountCommandResponse response,
        ICurrentUserContext currentUserContext,
        CancellationToken cancellationToken);
}
