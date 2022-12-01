namespace mark.davison.finance.web.features.Tag.Create;

public class CreateTagListCommandRequest : ICommand<CreateTagListCommandRequest, CreateTagListCommandResponse>
{
    public string Name { get; set; } = string.Empty;
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
}
