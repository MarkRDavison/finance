namespace mark.davison.finance.models.Entities;

public partial class Recurrence : FinanceEntity
{
    public Guid TransactionTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateOnly FirstDate { get; set; }
    public DateOnly? RepeatUntil { get; set; }
    public DateOnly LastDate { get; set; }
    public int Repetitions { get; set; }
    public bool ApplyRules { get; set; }
    public bool IsActive { get; set; } = true;
}

