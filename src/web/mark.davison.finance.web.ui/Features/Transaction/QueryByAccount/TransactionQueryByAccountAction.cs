namespace mark.davison.finance.web.ui.Features.Transaction.QueryByAccount;

public class TransactionQueryByAccountAction : IAction<TransactionQueryByAccountAction>
{
    public Guid AccountId { get; set; }
}
