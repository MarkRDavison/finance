namespace mark.davison.finance.models.dtos.Queries.TransactionByIdQuery;

public class TransactionByIdQueryResponse : Response
{
    public List<TransactionDto> Transactions { get; set; } = new();
}
