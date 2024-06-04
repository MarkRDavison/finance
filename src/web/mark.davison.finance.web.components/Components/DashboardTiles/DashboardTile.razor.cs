namespace mark.davison.finance.web.components.DashboardTiles;

public partial class DashboardTile
{
    [Parameter, EditorRequired]
    public required string Title { get; set; }

    [Parameter, EditorRequired]
    public required string Summary { get; set; }

    [Parameter]
    public string? Secondary { get; set; }

    [Parameter, EditorRequired]
    public required string Icon { get; set; }

    [Parameter, EditorRequired]
    public required string Colour { get; set; }
}