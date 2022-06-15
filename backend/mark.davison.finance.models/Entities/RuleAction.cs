namespace mark.davison.finance.models.Entities;

public partial class RuleAction : FinanceEntity
{
    public Guid RuleId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string ActionValue { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public bool StopProcessing { get; set; }
}

