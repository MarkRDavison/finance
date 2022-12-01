namespace mark.davison.finance.models.dtos.Commands.CreateTag;

[PostRequest(Path = "create-tag")]
public class CreateTagCommandRequest : ICommand<CreateTagCommandRequest, CreateTagCommandResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
}
