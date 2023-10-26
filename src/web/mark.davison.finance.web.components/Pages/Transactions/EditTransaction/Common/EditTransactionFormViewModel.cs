using mark.davison.finance.web.features.Category;

namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Common;

public class EditTransactionFormViewModel : IFormViewModel
{
    public Guid Id { get; set; }
    public Guid TransactionTypeId { get; set; }
    public DateTime? Date { get; set; }
    public bool Valid => false;

    public void AddSplit()
    {
        Items.Add(new EditTransactionFormViewModelItem
        {
            Id = Guid.NewGuid()
        });
    }

    public void RemoveSplit(Guid id)
    {
        var toRemove = Items.FirstOrDefault(_ => _.Id == id);

        if (toRemove != null)
        {
            Items.Remove(toRemove);
        }
    }

    public List<EditTransactionFormViewModelItem> Items { get; } = new();

    public IStateInstance<LookupState> LookupState { get; set; } = default!;
    public IStateInstance<AccountListState> AccountState { get; set; } = default!;
    public IStateInstance<CategoryListState> CategoryState { get; set; } = default!;
}
