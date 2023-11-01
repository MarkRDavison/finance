namespace mark.davison.finance.models.dtos.Queries.TransactionByIdQuery;

[GetRequest(Path = "transaction-by-id-query")]
public class TransactionByIdQueryRequest : IQuery<TransactionByIdQueryRequest, TransactionByIdQueryResponse>
{
    public Guid TransactionGroupId { get; set; } = new();
}
