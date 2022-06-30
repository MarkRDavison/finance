namespace mark.davison.zui.foundations.Abstractions;

public interface IDropdownItem
{
    public string Id { get; set; }
    public string? Code { get; set; }
    public string PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
}
public class DropdownItem : IDropdownItem
{
    public DropdownItem(string id, string primaryText)
    {
        Id = id;
        PrimaryText = primaryText;
    }

    public string Id { get; set; }
    public string? Code { get; set; }
    public string PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
}
