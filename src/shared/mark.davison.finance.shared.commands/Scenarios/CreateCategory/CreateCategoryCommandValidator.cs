namespace mark.davison.finance.shared.commands.Scenarios.CreateCategory.Validators;

public class CreateCategoryCommandValidator : ICommandValidator<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    private readonly IFinanceDbContext _dbContext;

    // TODO: Consolidate messages
    public const string VALIDATION_DUPLICATE_CATEGORY_NAME = "VALIDATION_DUPLICATE_CATEGORY_NAME";

    public CreateCategoryCommandValidator(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateCategoryCommandResponse> ValidateAsync(CreateCategoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateCategoryCommandResponse { };

        var duplicate = await _dbContext.Set<Category>()
            .Where(_ =>
                _.UserId == currentUserContext.CurrentUser.Id &&
                _.Name == request.Name)
            .AnyAsync(cancellationToken); // TODO: IDbContext<TContext>.AnyAsync<TEntity>(pred, cancellationtoken)

        if (duplicate)
        {
            response.Errors.Add(VALIDATION_DUPLICATE_CATEGORY_NAME);
        }

        return response;
    }
}
