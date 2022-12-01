namespace mark.davison.finance.models.dtos.Commands.CreateTag;

public class CreateTagCommandResponse // TODO: Common base class
{
    public bool Success { get; set; }
    public List<string> Error { get; set; } = new();
    public List<string> Warning { get; set; } = new();
}
