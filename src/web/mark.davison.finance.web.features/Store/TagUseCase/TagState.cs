namespace mark.davison.finance.web.features.Store.TagUseCase;

[FeatureState]
public sealed class TagState
{
    public TagState() : this([])
    {
    }

    public TagState(IEnumerable<TagDto> tags)
    {
        Tags = new(tags.ToList());
    }

    public ReadOnlyCollection<TagDto> Tags { get; }
}
