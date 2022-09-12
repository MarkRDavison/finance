namespace mark.davison.finance.web.ui.test.Helpers.State;

public static class CategoryListStateHelpers
{
    public static CategoryListState CreateCategoryListState(params CategoryListItemDto[] categoryListItems)
    {
        return new CategoryListState(categoryListItems);
    }
}
