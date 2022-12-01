namespace mark.davison.finance.bff.queries.Scenarios.TagListQuery;

public class TagListQueryHandler : IQueryHandler<TagListQueryRequest, TagListQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public TagListQueryHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<TagListQueryResponse> Handle(TagListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new TagListQueryResponse();

        var tags = await _httpRepository.GetEntitiesAsync<Tag>(
            new QueryParameters
            {
                { nameof(Tag.UserId), currentUserContext.CurrentUser.Id.ToString() }
            },
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
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
