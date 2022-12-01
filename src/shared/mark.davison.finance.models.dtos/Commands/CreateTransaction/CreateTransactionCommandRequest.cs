﻿namespace mark.davison.finance.models.dtos.Commands.CreateTransaction;

[PostRequest(Path = "create-transaction")]
public class CreateTransactionCommandRequest : ICommand<CreateTransactionCommandRequest, CreateTransactionCommandResponse>
{
    public Guid TransactionTypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<CreateTransactionDto> Transactions { get; set; } = new();
}
