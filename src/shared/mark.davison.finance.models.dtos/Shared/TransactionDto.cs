namespace mark.davison.finance.models.dtos.Shared;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public Guid TransactionJournalId { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? ForeignCurrencyId { get; set; }
    public string Description { get; set; } = string.Empty;
    public long Amount { get; set; }
    public long? ForeignAmount { get; set; }
    public bool Reconciled { get; set; }
}
