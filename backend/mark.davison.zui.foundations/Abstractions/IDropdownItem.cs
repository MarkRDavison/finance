namespace mark.davison.zui.foundations.Abstractions;

public interface IDropdownItem
{
    public string Id { get; set; }
    public string? Code { get; set; }
    public string PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
}
