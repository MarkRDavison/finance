namespace mark.davison.finance.bff.queries.Scenarios.CategoryListQuery;

public class CategoryListQueryHandler : IQueryHandler<CategoryListQueryRequest, CategoryListQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public CategoryListQueryHandler(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryListQueryResponse> Handle(CategoryListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CategoryListQueryResponse();

        var categories = await _dbContext
            .Set<Category>()
            .AsNoTracking()
            .Where(_ => _.UserId == currentUserContext.CurrentUser.Id)
            .Select(_ => new CategoryListItemDto
            {
                Id = _.Id,
                Name = _.Name
            })
            .ToListAsync(cancellationToken);

        response.Value = categories;

        return response;
    }
}
