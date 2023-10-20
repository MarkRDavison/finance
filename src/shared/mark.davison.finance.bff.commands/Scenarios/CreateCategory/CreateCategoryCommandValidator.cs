namespace mark.davison.finance.bff.commands.Scenarios.CreateCategory.Validators;

public class CreateCategoryCommandValidator : ICommandValidator<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    private readonly IRepository _repository;

    public const string VALIDATION_DUPLICATE_CATEGORY_NAME = "VALIDATION_DUPLICATE_CATEGORY_NAME";

    public CreateCategoryCommandValidator(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateCategoryCommandResponse> ValidateAsync(CreateCategoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateCategoryCommandResponse { };

        await using (_repository.BeginTransaction())
        {
            var duplicate = await _repository.EntityExistsAsync<Category>(
                _ =>
                    _.UserId == currentUserContext.CurrentUser.Id &&
                    _.Name == request.Name,
                cancellationToken);

            if (duplicate)
            {
                response.Errors.Add(VALIDATION_DUPLICATE_CATEGORY_NAME);
            }
        }

        return response;
    }
}
