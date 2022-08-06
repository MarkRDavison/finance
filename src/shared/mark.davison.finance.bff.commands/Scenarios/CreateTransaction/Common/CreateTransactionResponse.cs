namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionResponse // TODO: Base response class?
{
    public bool Success { get; set; }
    public List<string> Error { get; set; } = new();
    public List<string> Warning { get; set; } = new();

    public TransactionGroup? Group { get; set; }
    public List<TransactionJournal> Journals { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}
