namespace mark.davison.finance.accounting.rules;

public static class CurrencyRules
{
    public const int FinanceDecimalPlaces = 4;

    public static long ToPersisted(decimal value)
    {
        return (long)(value * (decimal)Math.Pow(10, FinanceDecimalPlaces));
    }
    public static decimal FromPersisted(long value)
    {
        return ((decimal)value / (decimal)Math.Pow(10, FinanceDecimalPlaces));
    }
}
