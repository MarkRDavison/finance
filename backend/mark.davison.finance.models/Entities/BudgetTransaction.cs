namespace mark.davison.finance.models.Entities;

public partial class BudgetTransaction : FinanceEntity
{
    public Guid BudgetId { get; set; }
    public Guid TransactionId { get; set; }
}