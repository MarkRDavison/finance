namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionCommandHandler : ICommandHandler<CreateTransactionRequest, CreateTransactionResponse>
{
    private readonly IHttpRepository _httpRepository;
    private readonly ICreateTransactionCommandValidator _validator;
    private readonly ICreateTransactionCommandProcessor _processor;

    public CreateTransactionCommandHandler(
        IHttpRepository httpRepository,
        ICreateTransactionCommandValidator validator,
        ICreateTransactionCommandProcessor processor
    )
    {
        _httpRepository = httpRepository;
        _validator = validator;
        _processor = processor;
    }

    public async Task<CreateTransactionResponse> Handle(CreateTransactionRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = await _validator.Validate(command, currentUserContext, cancellation);

        if (response.Success)
        {
            response = await _processor.Process(command, response, currentUserContext, _httpRepository, cancellation);
        }

        return response;
    }
}
