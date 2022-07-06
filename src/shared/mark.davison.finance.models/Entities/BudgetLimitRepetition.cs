namespace mark.davison.finance.models.Entities;

public partial class BudgetLimitRepetition : FinanceEntity
{
    public Guid BudgetLimitId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long Amount { get; set; }
}

