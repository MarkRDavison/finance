namespace mark.davison.finance.bff.commands.Scenarios.CreateLocation;

public class CreateAccountResponse
{
    public bool Success { get; set; }
    public List<string> Error { get; set; } = new();
    public List<string> Warning { get; set; } = new();
}
