namespace mark.davison.finance.models.Entities;

public partial class RuleGroup : FinanceEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public bool StopProcessing { get; set; }
}

