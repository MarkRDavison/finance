namespace mark.davison.finance.models.dtos.Commands.UpsertAccount;

[PostRequest(Path = "upsert-account")]
public class UpsertAccountCommandRequest : ICommand<UpsertAccountCommandRequest, UpsertAccountCommandResponse>
{
    public UpsertAccountDto UpsertAccountDto { get; set; } = new();
}