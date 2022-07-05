namespace mark.davison.zui.Components.Input;

public class BaseInput<TInputType> : Component
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
    public TInputType Value { get; set; } = default!;

    [Parameter]
    public EventCallback<TInputType> ValueChanged { get; set; }
}
