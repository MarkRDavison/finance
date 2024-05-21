namespace mark.davison.finance.web.features.Store.TransactionUseCase;

// TODO: Add loading flag
[FeatureState]
public class TransactionState
{
    public TransactionState() : this([])
    {
    }

    public TransactionState(IEnumerable<TransactionDto> tags)
    {
        Transactions = new(tags.ToList());
    }

    public ReadOnlyCollection<TransactionDto> Transactions { get; }
}
