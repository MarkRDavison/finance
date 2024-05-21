namespace mark.davison.finance.web.features.Store.CategoryUseCase;

public sealed class FetchCategoriesAction : BaseAction
{

}

public sealed class FetchCategoriesActionResponse : BaseActionResponse<List<CategoryListItemDto>>
{

}

public sealed class CreateCategoryAction : BaseAction
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class CreateCategoryActionResponse : BaseActionResponse<CategoryListItemDto>
{

}