namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount;

// TODO: Replace with ValidateAndProcessCommandHandler
public class UpsertAccountCommandHandler : ICommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>
{

    private readonly IUpsertAccountCommandValidator _upsertAccountCommandValidator;
    private readonly IUpsertAccountCommandProcessor _upsertAccountCommandProcessor;

    public UpsertAccountCommandHandler(
        IUpsertAccountCommandValidator createAccountCommandValidator,
        IUpsertAccountCommandProcessor upsertAccountCommandProcessor
    )
    {
        _upsertAccountCommandValidator = createAccountCommandValidator;
        _upsertAccountCommandProcessor = upsertAccountCommandProcessor;
    }

    public async Task<UpsertAccountCommandResponse> Handle(UpsertAccountCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _upsertAccountCommandValidator.Validate(request, currentUserContext, cancellationToken);

        if (!response.Success)
        {
            return response;
        }

        return await _upsertAccountCommandProcessor.Process(request, response, currentUserContext, cancellationToken);
    }
}
