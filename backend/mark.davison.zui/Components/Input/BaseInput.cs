namespace mark.davison.zui.Components.Input;

public class BaseInput : Component
{
    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public bool IsReadOnly { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public bool IsLabelDisplay { get; set; }

    [Parameter]
    public string Width { get; set; } = Size.DefaultControlWidth;

    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }
}
