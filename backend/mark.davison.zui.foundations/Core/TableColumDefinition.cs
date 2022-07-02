namespace mark.davison.zui.foundations.Core;

public class TableColumDefinition<TRow> where TRow : class
{
    public string Title { get; set; } = string.Empty;
    public string Width { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public Alignment Align { get; set; } = Alignment.Right;
}
