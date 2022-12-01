namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionCommandHandler : ICommandHandler<CreateTransactionCommandRequest, CreateTransactionCommandResponse>
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

    public async Task<CreateTransactionCommandResponse> Handle(CreateTransactionCommandRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _validator.Validate(command, currentUserContext, cancellationToken);

        if (response.Success)
        {
            response = await _processor.Process(command, response, currentUserContext, _httpRepository, cancellationToken);
        }

        return response;
    }
}
