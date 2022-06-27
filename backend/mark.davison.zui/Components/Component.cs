namespace mark.davison.zui.Components;

public class Component : ComponentBase
{

    [Parameter]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Parameter]
    public string CssClass { get; set; } = string.Empty;

}
