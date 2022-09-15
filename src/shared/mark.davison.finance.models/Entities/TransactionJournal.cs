namespace mark.davison.finance.models.Entities;

public partial class TransactionJournal : FinanceEntity
{
    public Guid TransactionTypeId { get; set; }
    public Guid TransactionGroupId { get; set; }
    public Guid? BillId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? ForeignCurrencyId { get; set; }
    public Guid? CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly Date { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly? InterestDate { get; set; }
    [JsonConverter(typeof(DateOnlyConverter))] // TODO: .net 7 plz...
    public DateOnly? ProcessDate { get; set; }
    public int Order { get; set; }
    public bool Completed { get; set; }

    public virtual TransactionType? TransactionType { get; set; }
    public virtual TransactionGroup? TransactionGroup { get; set; }
    public virtual List<Transaction> Transactions { get; set; } = new();
    public virtual Bill? Bill { get; set; }
    public virtual Currency? Currency { get; set; }
    public virtual Currency? ForeignCurrency { get; set; }
    public virtual Category? Category { get; set; }

}

