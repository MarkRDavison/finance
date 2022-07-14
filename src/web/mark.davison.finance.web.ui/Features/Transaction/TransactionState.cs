namespace mark.davison.finance.web.ui.Features.Transaction;

public class TransactionState : IState
{
    public TransactionState()
    {
        Transactions = Enumerable.Empty<TransactionDto>();
    }
    public TransactionState(IEnumerable<TransactionDto> transactions)
    {
        Transactions = transactions.ToList();
    }

    public IEnumerable<TransactionDto> Transactions { get; private set; }

    public void Initialise()
    {
        Transactions = Enumerable.Empty<TransactionDto>();
    }
}
