namespace mark.davison.finance.bff.queries.Scenarios.CategoryListQuery;

public class CategoryListQueryHandler : IQueryHandler<CategoryListQueryRequest, CategoryListQueryResponse>
{
    private readonly IRepository _repository;

    public CategoryListQueryHandler(IRepository httpRepository)
    {
        _repository = httpRepository;
    }

    public async Task<CategoryListQueryResponse> Handle(CategoryListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CategoryListQueryResponse();

        var categories = await _repository.GetEntitiesAsync<Category>(
            _ => _.UserId == currentUserContext.CurrentUser.Id,
            cancellationToken);

        response.Categories.AddRange(categories.Select(_ => new CategoryListItemDto
        {
            Id = _.Id,
            Name = _.Name
        }));

        return response;
    }
}
