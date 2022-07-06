namespace mark.davison.finance.models.Entities;

public partial class AutoBudget : FinanceEntity
{
    public Guid BudgetId { get; set; }
    public Guid CurrencyId { get; set; }
    public int AutoBudgetType { get; set; }
    public long Amount { get; set; }
    public string Period { get; set; } = string.Empty;
}

