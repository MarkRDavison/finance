namespace mark.davison.finance.models.Entities;

public partial class CategoryTransaction : FinanceEntity
{
    public Guid CategoryId { get; set; }
    public Guid TransactionId { get; set; }

    public virtual Category? Category { get; set; }
    public virtual Transaction? Transaction { get; set; }
}