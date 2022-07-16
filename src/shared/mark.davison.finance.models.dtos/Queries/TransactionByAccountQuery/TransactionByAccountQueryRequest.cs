namespace mark.davison.finance.models.dtos.Queries.TransactionByAccountQuery;

[GetRequest(Path = "transaction-by-account-query")]
public class TransactionByAccountQueryRequest : ICommand<TransactionByAccountQueryRequest, TransactionByAccountQueryResponse>
{
    public Guid AccountId { get; set; }
}
