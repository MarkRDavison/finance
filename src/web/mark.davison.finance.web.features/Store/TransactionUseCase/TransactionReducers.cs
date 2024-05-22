namespace mark.davison.finance.web.features.Store.TransactionUseCase;

public static class TransactionReducers
{
    [ReducerMethod]
    public static TransactionState FetchTransactionsByGroupResponse(TransactionState state, FetchTransactionsByGroupResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new TransactionState(Merge(state, response.Value));
        }

        return state;
    }

    [ReducerMethod]
    public static TransactionState FetchTransactionsByAccountResponse(TransactionState state, FetchTransactionsByAccountResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new TransactionState(Merge(state, response.Value));
        }

        return state;
    }

    [ReducerMethod]
    public static TransactionState CreateTransactionActionResponse(TransactionState state, CreateTransactionActionResponse response)
    {
        Console.WriteLine("CreateTransactionActionResponse reduced");
        if (response.Success)
        {
            return new TransactionState(Merge(state, response.Transactions));
        }

        return state;
    }

    private static List<TransactionDto> Merge(TransactionState state, List<TransactionDto> transactions)
    {
        var existingIds = state.Transactions.Select(_ => _.Id).ToHashSet();

        return [
            ..transactions,
            ..state.Transactions.Where(_ => !existingIds.Contains(_.Id))
        ];
    }
}
