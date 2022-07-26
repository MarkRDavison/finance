namespace mark.davison.finance.models.Entities;

public class FinanceEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
}

