namespace mark.davison.finance.models.Entities;

public partial class Goal : FinanceEntity
{
    public bool IsActive { get; set; } = true;
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long TargetAmount { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly TargetDate { get; set; }
}
