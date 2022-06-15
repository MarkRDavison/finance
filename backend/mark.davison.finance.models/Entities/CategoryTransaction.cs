namespace mark.davison.finance.models.Entities;

public partial class CategoryTransaction : FinanceEntity
{
    public Guid CategoryId { get; set; }
    public Guid TransactionId { get; set; }
}