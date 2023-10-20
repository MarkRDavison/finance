namespace mark.davison.finance.bff.commands.Scenarios.CreateTag;

public class CreateTagCommandHandler : ICommandHandler<CreateTagCommandRequest, CreateTagCommandResponse>
{
    private readonly IRepository _repository;
    private readonly ICreateTagCommandValidator _createTagCommandValidator;

    public CreateTagCommandHandler(
        IRepository repository,
        ICreateTagCommandValidator createTagCommandValidator
    )
    {
        _repository = repository;
        _createTagCommandValidator = createTagCommandValidator;
    }

    public async Task<CreateTagCommandResponse> Handle(CreateTagCommandRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _createTagCommandValidator.Validate(command, currentUserContext, cancellationToken);

        if (!response.Success)
        {
            return response;
        }

        // TODO: Replace with validator/processor pattern
        var category = new Tag
        {
            Id = command.Id,
            Name = command.Name,
            UserId = currentUserContext.CurrentUser.Id
        };

        var tag = await _repository.UpsertEntityAsync(
            category,
            cancellationToken);

        if (tag == null)
        {
            response.Error.Add("DB_UPSERT_ERROR"); // TODO: Standard db errors
        }

        return response;
    }
}
