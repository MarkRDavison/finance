namespace mark.davison.finance.models.Entities;

public partial class Goal : FinanceEntity
{
    public bool IsActive { get; set; } = true;
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long TargetAmount { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly StartDate { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly TargetDate { get; set; }
}
