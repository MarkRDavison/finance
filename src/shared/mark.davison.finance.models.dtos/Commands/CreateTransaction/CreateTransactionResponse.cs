namespace mark.davison.finance.models.dtos.Commands.CreateTransaction;

public class CreateTransactionResponse : Response // TODO: Base response class?
{

    public TransactionGroupDto? Group { get; set; }
    public List<TransactionJournalDto> Journals { get; set; } = new();
    public List<TransactionDto> Transactions { get; set; } = new();
}
