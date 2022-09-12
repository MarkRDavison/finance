namespace mark.davison.finance.web.features.Category;

public class CategoryListState : IState
{
    public CategoryListState() : this(Enumerable.Empty<CategoryListItemDto>())
    {
    }

    public CategoryListState(IEnumerable<CategoryListItemDto> accounts)
    {
        Categories = accounts.ToList();
        LastModified = DateTime.Now;
    }

    public IEnumerable<CategoryListItemDto> Categories { get; private set; }
    public DateTime LastModified { get; private set; }

    public void Initialise()
    {
        Categories = Enumerable.Empty<CategoryListItemDto>();
        LastModified = default;
    }
}
