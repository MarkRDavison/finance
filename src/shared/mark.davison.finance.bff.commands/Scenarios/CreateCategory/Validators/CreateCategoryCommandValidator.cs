namespace mark.davison.finance.bff.commands.Scenarios.CreateCategory.Validators;

public class CreateCategoryCommandValidator : ICreateCategoryCommandValidator
{
    private readonly IHttpRepository _httpRepository;

    public const string VALIDATION_DUPLICATE_CATEGORY_NAME = "VALIDATION_DUPLICATE_CATEGORY_NAME";

    public CreateCategoryCommandValidator(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<CreateCategoryResponse> Validate(CreateCategoryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = new CreateCategoryResponse { };

        var duplicate = await _httpRepository.GetEntityAsync<Category>(
            new QueryParameters
            {
                { nameof(Category.Name), request.Name },
                { nameof(Category.UserId), currentUserContext.CurrentUser.Id.ToString() }
            },
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        if (duplicate != null)
        {
            response.Error.Add(VALIDATION_DUPLICATE_CATEGORY_NAME);
        }

        response.Success = !response.Error.Any();
        return response;
    }
}
