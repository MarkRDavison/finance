namespace mark.davison.finance.models.Entities;

public partial class CurrencyExchangeRate : FinanceEntity
{
    public Guid FromCurrencyId { get; set; }
    public Guid ToCurrencyId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Rate { get; set; } // TODO: Use long here?
    public decimal UserRate { get; set; } // TODO: Use long here?
}

