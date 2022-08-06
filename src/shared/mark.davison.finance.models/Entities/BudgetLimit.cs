namespace mark.davison.finance.models.Entities;

public partial class BudgetLimit : FinanceEntity
{
    public Guid BudgetId { get; set; }
    public Guid CurrencyId { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly StartDate { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly EndDate { get; set; }
    public long Amount { get; set; }
    public string Period { get; set; } = string.Empty;
    public bool Generated { get; set; }
}

