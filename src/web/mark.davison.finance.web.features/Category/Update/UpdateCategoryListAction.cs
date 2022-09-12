namespace mark.davison.finance.web.features.Category.Update;

public class UpdateCategoryListAction : IAction<UpdateCategoryListAction>
{

    public UpdateCategoryListAction(IEnumerable<CategoryListItemDto> items)
    {
        Items = items;
    }

    public IEnumerable<CategoryListItemDto> Items { get; }
}
