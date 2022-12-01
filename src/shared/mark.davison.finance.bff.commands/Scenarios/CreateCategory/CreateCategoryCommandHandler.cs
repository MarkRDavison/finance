namespace mark.davison.finance.bff.commands.Scenarios.CreateCategory;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    private readonly IHttpRepository _httpRepository;
    private readonly ICreateCategoryCommandValidator _createCategoryCommandValidator;

    public CreateCategoryCommandHandler(
        IHttpRepository httpRepository,
        ICreateCategoryCommandValidator createCategoryCommandValidator
    )
    {
        _httpRepository = httpRepository;
        _createCategoryCommandValidator = createCategoryCommandValidator;
    }

    public async Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommandRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _createCategoryCommandValidator.Validate(command, currentUserContext, cancellationToken);

        if (!response.Success)
        {
            return response;
        }

        var category = new Category
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
