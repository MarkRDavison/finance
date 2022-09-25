namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount;

public class UpsertAccountCommandHandler : ICommandHandler<UpsertAccountRequest, UpsertAccountResponse>
{

    private readonly IHttpRepository _httpRepository;
    private readonly IUpsertAccountCommandValidator _upsertAccountCommandValidator;
    private readonly IUpsertAccountCommandProcessor _upsertAccountCommandProcessor;

    public UpsertAccountCommandHandler(
        IHttpRepository httpRepository,
        IUpsertAccountCommandValidator createAccountCommandValidator,
        IUpsertAccountCommandProcessor upsertAccountCommandProcessor
    )
    {
        _httpRepository = httpRepository;
        _upsertAccountCommandValidator = createAccountCommandValidator;
        _upsertAccountCommandProcessor = upsertAccountCommandProcessor;
    }

    public async Task<UpsertAccountResponse> Handle(UpsertAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _upsertAccountCommandValidator.Validate(request, currentUserContext, cancellationToken);

        if (!response.Success)
        {
            return response;
        }

        return await _upsertAccountCommandProcessor.Process(request, response, currentUserContext, _httpRepository, cancellationToken);
    }
}
