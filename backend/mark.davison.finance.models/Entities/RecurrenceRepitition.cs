namespace mark.davison.finance.models.Entities;

public partial class RecurrenceRepitition : FinanceEntity
{
    public Guid RecurrenceId { get; set; }
    public string RepititionType { get; set; } = string.Empty;
    public string RepititionMoment { get; set; } = string.Empty;
    public int RepititionSkip { get; set; }
    public int Weekend { get; set; }
}

