namespace mark.davison.finance.models.Entities;

public partial class TransactionJournal : FinanceEntity
{
    public Guid TransactionTypeId { get; set; }
    public Guid TransactionGroupId { get; set; }
    public Guid? BillId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? ForeignCurrencyId { get; set; }
    public string Description { get; set; } = string.Empty;
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly Date { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly? InterestDate { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly? ProcessDate { get; set; }
    public int Order { get; set; }
    public bool Completed { get; set; }

}

