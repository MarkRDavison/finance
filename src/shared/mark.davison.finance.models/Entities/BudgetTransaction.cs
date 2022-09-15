namespace mark.davison.finance.models.Entities;

public partial class BudgetTransaction : FinanceEntity
{
    public Guid BudgetId { get; set; }
    public Guid TransactionId { get; set; }

    public virtual Budget? Budget { get; set; }
    public virtual Transaction? Transaction { get; set; }
}