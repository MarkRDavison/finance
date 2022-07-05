using Microsoft.AspNetCore.Components;

namespace mark.davison.zui.foundations.Core;

public class TableColumDefinition<TRow> where TRow : class
{
    public string Title { get; set; } = string.Empty;
    public string Width { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public Alignment Align { get; set; } = Alignment.Right;
    public TableCellType Type { get; set; } = TableCellType.String;
    public Func<TRow, RenderFragment>? Template { get; set; }
    public Func<TRow, string>? Formatter { get; set; }
}
