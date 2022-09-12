namespace mark.davison.finance.bff.commands.Scenarios.CreateCategory.Validators;

public interface ICreateCategoryCommandValidator
{
    Task<CreateCategoryResponse> Validate(CreateCategoryRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellation);
}
