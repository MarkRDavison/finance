namespace mark.davison.finance.models.Entities;

public partial class BudgetLimitRepetition : FinanceEntity
{
    public Guid BudgetLimitId { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly StartDate { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly EndDate { get; set; }
    public long Amount { get; set; }
}

