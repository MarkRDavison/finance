namespace mark.davison.finance.web.features.Tag.Update;

public class UpdateTagListAction : IAction<UpdateTagListAction>
{

    public UpdateTagListAction(IEnumerable<TagDto> items)
    {
        Items = items;
    }

    public IEnumerable<TagDto> Items { get; }
}
