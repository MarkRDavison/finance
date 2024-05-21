namespace mark.davison.finance.web.features.Store.CategoryUseCase;

[FeatureState]
public class CategoryState
{
    public CategoryState() : this([])
    {
    }

    public CategoryState(IEnumerable<CategoryListItemDto> categories)
    {
        Categories = new(categories.ToList());
    }

    public ReadOnlyCollection<CategoryListItemDto> Categories { get; }
}
