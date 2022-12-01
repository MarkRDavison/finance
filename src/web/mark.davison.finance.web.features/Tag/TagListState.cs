namespace mark.davison.finance.web.features.Tag;

public class TagListState : IState
{
    public TagListState() : this(Enumerable.Empty<TagDto>())
    {
    }

    public TagListState(IEnumerable<TagDto> tags)
    {
        Tags = tags.ToList();
        LastModified = DateTime.Now;
    }

    public IEnumerable<TagDto> Tags { get; private set; }
    public DateTime LastModified { get; private set; }

    public void Initialise()
    {
        Tags = Enumerable.Empty<TagDto>();
        LastModified = default;
    }
}
