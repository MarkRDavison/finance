namespace mark.davison.zui.foundations.Abstractions;

public interface IDropdownItem<TId>
{
    public TId Id { get; set; }
    public string? Code { get; set; }
    public string PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
}
public class DropdownItem<TId> : IDropdownItem<TId>
{
    public DropdownItem(TId id, string primaryText)
    {
        Id = id;
        PrimaryText = primaryText;
    }

    public TId Id { get; set; }
    public string? Code { get; set; }
    public string PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
}
