namespace mark.davison.finance.models.dtos.Queries.TransactionByAccountQuery;

public class TransactionByAccountQueryResponse
{
    public List<TransactionDto> Transactions { get; set; } = new();
}
