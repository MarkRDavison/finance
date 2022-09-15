namespace mark.davison.finance.models.Entities;

public partial class BudgetTransactionJournal : FinanceEntity
{
    public Guid BudgetId { get; set; }
    public Guid? BudgetLimitId { get; set; }
    public Guid TransactionJournalId { get; set; }

    public virtual Budget? Budget { get; set; }
    public virtual BudgetLimit? BudgetLimit { get; set; }
    public virtual TransactionJournal? TransactionJournal { get; set; }
}