namespace mark.davison.finance.models.Entities;

public partial class BudgetLimit : FinanceEntity
{
    public Guid BudgetId { get; set; }
    public Guid CurrencyId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long Amount { get; set; }
    public string Period { get; set; } = string.Empty;
    public bool Generated { get; set; }

    public virtual Budget? Budget { get; set; }
    public virtual Currency? Currency { get; set; }
}

