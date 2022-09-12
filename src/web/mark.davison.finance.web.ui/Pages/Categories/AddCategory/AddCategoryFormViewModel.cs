namespace mark.davison.finance.web.ui.Pages.Categories.AddCategory;

public class AddCategoryFormViewModel
{
    public string Name { get; set; } = string.Empty;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        !CategoryListState.Instance.Categories.Any(_ => string.Equals(_.Name, Name, StringComparison.OrdinalIgnoreCase));

    public IStateInstance<CategoryListState> CategoryListState { get; set; } = default!;
}
