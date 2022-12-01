namespace mark.davison.finance.bff.commands.Scenarios.CreateTag.Validators;

public class CreateTagCommandValidator : ICreateTagCommandValidator
{
    private readonly IHttpRepository _httpRepository;
    public const string VALIDATION_DUPLICATE_TAG_NAME = "VALIDATION_DUPLICATE_TAG_NAME";

    public CreateTagCommandValidator(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<CreateTagCommandResponse> Validate(CreateTagCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTagCommandResponse { };

        var duplicate = await _httpRepository.GetEntityAsync<Tag>(
            new QueryParameters
            {
                { nameof(Tag.Name), request.Name },
                { nameof(Tag.UserId), currentUserContext.CurrentUser.Id.ToString() }
            },
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellationToken);

        if (duplicate != null)
        {
            response.Error.Add(VALIDATION_DUPLICATE_TAG_NAME);
        }

        response.Success = !response.Error.Any();
        return response;
    }
}
