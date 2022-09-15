namespace mark.davison.finance.models.Entities;

public partial class RuleTrigger : FinanceEntity
{
    public Guid RuleId { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public string TriggerValue { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public bool StopProcessing { get; set; }

    public virtual Rule? Rule { get; set; }
}

