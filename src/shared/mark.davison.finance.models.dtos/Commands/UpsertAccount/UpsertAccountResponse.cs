namespace mark.davison.finance.models.dtos.Commands.UpsertAccount;

public class UpsertAccountResponse // TODO: Base response class?
{
    public bool Success { get; set; }
    public List<string> Error { get; set; } = new();
    public List<string> Warning { get; set; } = new();
}
