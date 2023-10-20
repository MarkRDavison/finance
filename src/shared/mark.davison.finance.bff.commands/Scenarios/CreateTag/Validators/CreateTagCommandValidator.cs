namespace mark.davison.finance.bff.commands.Scenarios.CreateTag.Validators;

public class CreateTagCommandValidator : ICreateTagCommandValidator
{
    private readonly IRepository _repository;
    public const string VALIDATION_DUPLICATE_TAG_NAME = "VALIDATION_DUPLICATE_TAG_NAME";

    public CreateTagCommandValidator(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTagCommandResponse> Validate(CreateTagCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTagCommandResponse { };

        var duplicate = await _repository.GetEntityAsync<Tag>(
            _ => _.UserId == currentUserContext.CurrentUser.Id && _.Name == request.Name,
            cancellationToken);

        if (duplicate != null)
        {
            response.Error.Add(VALIDATION_DUPLICATE_TAG_NAME);
        }

        response.Success = !response.Error.Any();
        return response;
    }
}
