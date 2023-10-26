namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Common;

public class EditTransactionFormViewModelItem
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? SourceAccountId { get; set; }
    public Guid? DestinationAccountId { get; set; }
    public decimal? Amount { get; set; }
    public Guid? CategoryId { get; set; }
}
