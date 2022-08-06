namespace mark.davison.finance.models.Entities;

public partial class GoalEvent : FinanceEntity
{
    public Guid GoalId { get; set; }
    public Guid TransactionJournalId { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly Date { get; set; }
    public long Amount { get; set; }
}

