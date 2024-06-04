namespace mark.davison.finance.web.components.CommonCandidates.Activity;

public partial class ActivityMonitor
{
    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? LoadingContent { get; set; }

    [Parameter]
    public MudBlazor.Size Size { get; set; } = MudBlazor.Size.Large;
}