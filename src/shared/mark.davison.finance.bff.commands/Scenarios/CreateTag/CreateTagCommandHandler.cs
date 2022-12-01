namespace mark.davison.finance.bff.commands.Scenarios.CreateTag;

public class CreateTagCommandHandler : ICommandHandler<CreateTagCommandRequest, CreateTagCommandResponse>
{
    private readonly IHttpRepository _httpRepository;
    private readonly ICreateTagCommandValidator _createTagCommandValidator;

    public CreateTagCommandHandler(
        IHttpRepository httpRepository,
        ICreateTagCommandValidator createTagCommandValidator
    )
    {
        _httpRepository = httpRepository;
        _createTagCommandValidator = createTagCommandValidator;
    }

    public async Task<CreateTagCommandResponse> Handle(CreateTagCommandRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _createTagCommandValidator.Validate(command, currentUserContext, cancellationToken);

        if (!response.Success)
        {
            return response;
        }

        var category = new Tag
        {
            Id = command.Id,
            Name = command.Name,
            UserId = currentUserContext.CurrentUser.Id
        };

        await _httpRepository.UpsertEntityAsync(
            category,
            HeaderParameters.Auth(
                currentUserContext.Token,
                currentUserContext.CurrentUser),
            cancellationToken);

        return response;
    }
}
