namespace mark.davison.finance.web.features.Transaction.Update;

public class UpdateTransactionStateItemsAction : IAction<UpdateTransactionStateItemsAction>
{
    public UpdateTransactionStateItemsAction(IEnumerable<TransactionDto> items)
    {
        Items = items;
    }

    public IEnumerable<TransactionDto> Items { get; }
}
