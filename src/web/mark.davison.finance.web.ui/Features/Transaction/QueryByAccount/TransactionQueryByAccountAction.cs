namespace mark.davison.finance.web.ui.Features.Transaction.QueryByAccount;

public class TransactionQueryByAccountAction : IAction<TransactionQueryByAccountAction>
{
    public TransactionQueryByAccountAction(IEnumerable<TransactionDto> transactions)
    {
        Transactions = transactions.ToList();
    }

    public List<TransactionDto> Transactions { get; }
}
