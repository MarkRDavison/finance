namespace mark.davison.finance.web.features.Store.TagUseCase;

public sealed class FetchTagsAction : BaseAction
{

}

public sealed class FetchTagsActionResponse : BaseActionResponse<List<TagDto>>
{

}

public sealed class CreateTagAction : BaseAction
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class CreateTagActionResponse : BaseActionResponse<TagDto>
{

}