namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Common;

public class EditTransactionFormViewModel : IFormViewModel
{
    public Guid Id { get; set; }
    public Guid TransactionTypeId { get; set; }
    public bool HideTransactionType { get; set; }
    public bool Valid => false;
}
