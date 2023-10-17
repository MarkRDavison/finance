namespace mark.davison.finance.web.components.CommonCandidates.Components.Currency;

public partial class CurrencyField
{
    private ICurrencyInfo? _currencyInfo = default!;

    [Parameter, EditorRequired]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BL0007:Component parameters should be auto properties", Justification = "<Pending>")]
    public required ICurrencyInfo? CurrencyInfo
    {
        get => _currencyInfo;
        set
        {
            if (_currencyInfo != value)
            {
                _currencyInfo = value;
                UpdateDisplayText();
            }
        }
    }

    private string formatter(decimal num)
    {
        if (num < 0)
        {
            return $"{CurrencyInfo?.Symbol ?? string.Empty}({num.ToString($"N{CurrencyInfo?.DecimalPlaces ?? 0}")})";
        }
        return (CurrencyInfo?.Symbol ?? string.Empty) + num.ToString($"N{CurrencyInfo?.DecimalPlaces ?? 0}");
    }

    protected override void OnInitialized()
    {
        this.Formatter = formatter;
        if (CurrencyInfo != null)
        {
            this.DecimalPlaces = CurrencyInfo.DecimalPlaces;
        }
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        this.Formatter = formatter;
        if (CurrencyInfo != null)
        {
            this.DecimalPlaces = CurrencyInfo.DecimalPlaces;
        }
        base.OnParametersSet();
    }
}
