namespace mark.davison.finance.models.dtos.Shared;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
}
