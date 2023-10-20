namespace mark.davison.finance.models.dtos.Commands.CreateTransaction;

[PostRequest(Path = "create-transaction")]
public class CreateTransactionRequest : ICommand<CreateTransactionRequest, CreateTransactionResponse>
{
    public Guid TransactionTypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<CreateTransactionDto> Transactions { get; set; } = new();
}
