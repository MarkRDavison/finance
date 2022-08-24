namespace mark.davison.finance.web.features.Transaction.Create;

public class TransactionCreateCommand : ICommand<TransactionCreateCommand, TransactionCreateCommandResponse>
{
    public string Description { get; set; } = string.Empty;
    public Guid TransactionTypeId { get; set; }

    public List<CreateTransactionDto> CreateTransactionDtos { get; set; } = new();
}
