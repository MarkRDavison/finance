namespace mark.davison.finance.web.components.CommonCandidates.Activity;

public partial class ActivityMonitor
{
    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public RenderFragment? LoadingContent { get; set; }

    protected override void OnParametersSet()
    {
        Console.WriteLine("ActivityMonitor:OnParameterSet: Loading:{0}", Loading);
    }

    [Parameter]
    public Size Size { get; set; } = Size.Large;
}