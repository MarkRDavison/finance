namespace mark.davison.finance.models.dtos.Commands.UpsertAccount;

[PostRequest(Path = "upsert-account")]
public class UpsertAccountRequest : ICommand<UpsertAccountRequest, UpsertAccountResponse>
{
    public UpsertAccountDto UpsertAccountDto { get; set; } = new();
}