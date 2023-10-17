namespace mark.davison.finance.web.components.CommonCandidates.Components.Currency;
public interface ICurrencyInfo
{
    public string Symbol { get; }
    public int DecimalPlaces { get; }
}

public class CurrencyInfo : ICurrencyInfo
{
    public CurrencyInfo(string symbol, int decimalPlaces)
    {
        Symbol = symbol;
        DecimalPlaces = decimalPlaces;
    }

    public string Symbol { get; }
    public int DecimalPlaces { get; }
}