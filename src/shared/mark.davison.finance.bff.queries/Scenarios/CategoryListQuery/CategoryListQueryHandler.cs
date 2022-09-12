namespace mark.davison.finance.bff.queries.Scenarios.CategoryListQuery;

public class CategoryListQueryHandler : IQueryHandler<CategoryListQueryRequest, CategoryListQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public CategoryListQueryHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<CategoryListQueryResponse> Handle(CategoryListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = new CategoryListQueryResponse();

        var categories = await _httpRepository.GetEntitiesAsync<Category>(
            new QueryParameters
            {
                { nameof(Category.UserId), currentUserContext.CurrentUser.Id.ToString() }
            },
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        response.Categories.AddRange(categories.Select(_ => new CategoryListItemDto
        {
            Id = _.Id,
            Name = _.Name
        }));

        return response;
    }
}
