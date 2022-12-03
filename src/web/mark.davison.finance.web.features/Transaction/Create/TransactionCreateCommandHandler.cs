﻿namespace mark.davison.finance.web.features.Transaction.Create;

public class TransactionCreateCommandHandler : ICommandHandler<TransactionCreateCommand, TransactionCreateCommandResponse>
{
    private readonly IClientHttpRepository _repository;

    public TransactionCreateCommandHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }
    public async Task<TransactionCreateCommandResponse> Handle(TransactionCreateCommand command, CancellationToken cancellationToken)
    {
        var request = new CreateTransactionCommandRequest
        {
            Description = command.Description,
            TransactionTypeId = command.TransactionTypeId,
            Transactions = command.CreateTransactionDtos
        };

        var response = await _repository.Post<CreateTransactionCommandResponse, CreateTransactionCommandRequest>(request, cancellationToken);

        return new TransactionCreateCommandResponse
        {
            Success = response.Success
        };
    }
}
