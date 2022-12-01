namespace mark.davison.finance.web.ui.Pages.Tags.AddTag;

public class AddTagFormViewModel
{
    public string Name { get; set; } = string.Empty;

    public bool Valid =>
        !string.IsNullOrEmpty(Name) &&
        !TagListState.Instance.Tags.Any(_ => string.Equals(_.Name, Name, StringComparison.OrdinalIgnoreCase));

    public IStateInstance<TagListState> TagListState { get; set; } = default!;
}
