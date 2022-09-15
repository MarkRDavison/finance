namespace mark.davison.finance.models.Entities;

public partial class AvailableBudget : FinanceEntity
{
    public Guid CurrencyId { get; set; }
    public long Amount { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly StartDate { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly EndDate { get; set; }

    public virtual Currency? Currency { get; set; }
}

