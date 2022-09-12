namespace mark.davison.finance.models.dtos.Commands.CreateTransaction;

public class CreateTransactionResponse // TODO: Base response class?
{
    public bool Success { get; set; }
    public List<string> Error { get; set; } = new();
    public List<string> Warning { get; set; } = new();

    public TransactionGroupDto? Group { get; set; }
    public List<TransactionJournalDto> Journals { get; set; } = new();
    public List<TransactionDto> Transactions { get; set; } = new();
}
