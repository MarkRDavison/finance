namespace mark.davison.finance.models.dtos.Shared;

public class CreateTransactionDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public DateOnly Date { get; set; }
    public long Amount { get; set; }
    public long? ForeignAmount { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid? ForeignCurrencyId { get; set; }
    public Guid? BudgetId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BillId { get; set; }
}
