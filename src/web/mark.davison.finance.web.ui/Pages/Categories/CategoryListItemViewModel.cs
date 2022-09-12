namespace mark.davison.finance.web.ui.Pages.Categories;

public class CategoryListItemViewModel : ITableRow<Guid>
{
    public Guid Id { get; set; }
    public LinkDefinition? Name { get; set; }
}
