namespace mark.davison.finance.bff.commands.Scenarios.CreateCategory.Validators;

public interface ICreateCategoryCommandValidator
{
    Task<CreateCategoryCommandResponse> Validate(CreateCategoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken);
}
