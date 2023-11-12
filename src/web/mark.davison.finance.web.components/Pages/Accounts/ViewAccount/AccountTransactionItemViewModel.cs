using mark.davison.finance.models.dtos.Shared;

namespace mark.davison.finance.web.components.Pages.Accounts.ViewAccount;

public class AccountTransactionItemViewModel
{
    public string? SplitDescription { get; set; }
    public bool IsSplit { get; set; }
    public Guid TransactionGroupId { get; set; }
    public List<AccountTransactionItemTransactionViewModel> Transactions { get; set; } = new();
}

public class AccountTransactionItemTransactionViewModel
{
    public TransactionDto SourceTransaction { get; set; } = default!;
    public TransactionDto DestinationTransaction { get; set; } = default!;
}