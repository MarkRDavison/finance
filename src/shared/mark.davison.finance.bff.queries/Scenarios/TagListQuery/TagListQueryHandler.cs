namespace mark.davison.finance.bff.queries.Scenarios.TagListQuery;

public class TagListQueryHandler : IQueryHandler<TagListQueryRequest, TagListQueryResponse>
{
    private readonly IRepository _repository;

    public TagListQueryHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<TagListQueryResponse> Handle(TagListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new TagListQueryResponse();

        var tags = await _repository.GetEntitiesAsync<Tag>(
            _ => _.UserId == currentUserContext.CurrentUser.Id,
            cancellationToken);

        response.Tags.AddRange(tags.Select(_ => new TagDto
        {
            Id = _.Id,
            Name = _.Name,
            MaxDate = _.MaxDate,
            MinDate = _.MinDate,
        }));

        return response;
    }
}
