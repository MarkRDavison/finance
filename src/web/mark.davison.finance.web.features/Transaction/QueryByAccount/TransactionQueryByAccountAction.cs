namespace mark.davison.finance.web.features.Transaction.QueryByAccount;

public class TransactionQueryByAccountAction : IAction<TransactionQueryByAccountAction>
{
    public Guid AccountId { get; set; }
}
