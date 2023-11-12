namespace mark.davison.finance.web.components.Pages.Accounts.ViewAccount;

public class ViewAccountGridRow
{
    public bool IsSplit { get; set; }
    public bool IsSubTransaction { get; set; }
    public LinkInfo Description { get; set; } = new();
    public Guid TransactionGroupId { get; set; }
    public decimal? Amount { get; set; }
    public DateOnly? Date { get; set; }
    public LinkInfo SourceAccount { get; set; } = new();
    public LinkInfo DestinationAccount { get; set; } = new();
    public string TransactionType { get; set; } = string.Empty;
    public LinkInfo Category { get; set; } = new();
}
