namespace mark.davison.finance.web.features.Store.TransactionUseCase;

public sealed class FetchTransactionsByGroup : BaseAction
{
    public Guid TransactionGroupId { get; set; }
}

public sealed class FetchTransactionsByGroupResponse : BaseActionResponse<List<TransactionDto>>
{

}

public sealed class FetchTransactionsByAccount : BaseAction
{
    public Guid AccountId { get; set; }
}

public sealed class FetchTransactionsByAccountResponse : BaseActionResponse<List<TransactionDto>>
{

}