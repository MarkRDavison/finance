namespace mark.davison.finance.bff.queries.Scenarios.TagListQuery;

public class TagListQueryHandler : IQueryHandler<TagListQueryRequest, TagListQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public TagListQueryHandler(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TagListQueryResponse> Handle(TagListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new TagListQueryResponse();

        var tags = await _dbContext
            .Set<Tag>()
            .AsNoTracking()
            .Select(_ => new TagDto
            {
                Id = _.Id,
                Name = _.Name,
                MaxDate = _.MaxDate,
                MinDate = _.MinDate,
            })
            .ToListAsync(cancellationToken);

        response.Tags.AddRange(tags);

        return response;
    }
}
