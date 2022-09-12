namespace mark.davison.finance.web.ui.Pages.Transactions.AddTransaction;

public class AddTransactionFormModel : IFormModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = string.Empty;
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public Guid ForeignCurrencyId { get; set; }
    public decimal ForeignAmount { get; set; }
    public Guid CategoryId { get; set; }
}
