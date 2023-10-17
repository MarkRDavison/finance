using Microsoft.AspNetCore.Components.Web;

namespace mark.davison.finance.web.components.CommonCandidates.Components.Numeric;

public partial class NumericField
{
    private bool _touched;
    private bool _preventKeyDown;

    public string StringValue { get; set; } = string.Empty;

    [Parameter]
    public decimal Value { get; set; }

    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<decimal> ValueChanged { get; set; }

    [Parameter]
    public int DecimalPlaces { get; set; } = 2;

    [Parameter]
    public bool Negative { get; set; } = true;

    [Parameter]
    public decimal? Min { get; set; }

    [Parameter]
    public decimal? Max { get; set; }

    [Parameter]
    public Func<decimal, string>? Formatter { get; set; }

    private void OnKeydown(KeyboardEventArgs args)
    {
        _preventKeyDown = false;

        if (args.Key.Length == 1 && args.CtrlKey)
        {
            return;
        }

        if (args.Key.Length != 1)
        {
            return;
        }

        if (!UpdateValue(StringValue + args.Key))
        {
            _preventKeyDown = true;
            return;
        }
    }

    private bool UpdateValue(string value)
    {
        var hyphenIndexFirst = value.IndexOf('-');
        var hyphenIndexSecondary = value.Length > 1 ? value.IndexOf('-', 1) : -1;

        if (!Negative && hyphenIndexFirst != -1)
        {
            return false;
        }

        if (hyphenIndexSecondary != -1)
        {
            return false;
        }
        var plusIndex = value.IndexOf('+', 1);

        if (plusIndex != -1)
        {
            return false;
        }

        var decimalIndexFirst = value.IndexOf('.');

        if (DecimalPlaces <= 0 && decimalIndexFirst != -1)
        {
            return false;
        }
        var decimalIndexSecondary =
          decimalIndexFirst == -1 ? -1 : value.IndexOf('.', decimalIndexFirst + 1);

        if (decimalIndexSecondary != -1)
        {
            return false;
        }

        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if ((c < '0' || c > '9') && c != '.' && c != '-' && c != '+')
            {
                return false;
            }
        }

        if (
          decimalIndexFirst != -1 &&
          value.Length - decimalIndexFirst - 1 > DecimalPlaces
        )
        {
            return false;
        }

        if (decimal.TryParse(value, out var numeric))
        {
            if (Max != null && numeric > Max)
            {
                return false;
            }

            if (Min != null && numeric < Min)
            {
                return false;
            }

            _touched = true;
            return true;
        }

        if (value == "-")
        {
            _touched = true;
            return true;
        }

        return false;
    }

    private void OnInput(ChangeEventArgs e)
    {
        var newValue = e.Value as string;
        if (newValue != null)
        {
            StringValue = newValue;
        }
    }

    private void OnFocus()
    {
        if (_touched || !string.IsNullOrEmpty(StringValue))
        {
            StringValue = Value.ToString();
        }
    }

    protected void UpdateDisplayText()
    {
        if (Formatter != null && !string.IsNullOrEmpty(StringValue))
        {
            StringValue = Formatter(Value);
        }
    }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(StringValue) && !_touched && Value != 0.0M)
        {
            StringValue = Value.ToString($"N{DecimalPlaces}");
        }
        base.OnParametersSet();
    }

    private async Task OnBlur()
    {
        if (!string.IsNullOrEmpty(StringValue))
        {
            if (decimal.TryParse(StringValue, out var newValue))
            {
                Value = newValue;
                await ValueChanged.InvokeAsync(Value);
                UpdateDisplayText();
            }
        }
    }
}
