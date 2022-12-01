namespace mark.davison.finance.web.ui.Pages.Tags;

public class TagListItemViewModel : ITableRow<Guid>
{
    public Guid Id { get; set; }
    public LinkDefinition? Name { get; set; }
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
}
