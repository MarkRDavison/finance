namespace mark.davison.finance.bff.commands.Scenarios.CreateTag.Validators;

public class CreateTagCommandValidator : ICreateTagCommandValidator
{
    private readonly IFinanceDbContext _dbContext;

    public const string VALIDATION_DUPLICATE_TAG_NAME = "VALIDATION_DUPLICATE_TAG_NAME";

    public CreateTagCommandValidator(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateTagCommandResponse> Validate(CreateTagCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTagCommandResponse { };

        var duplicate = await _dbContext.Set<Tag>()
            .Where(_ =>
                _.UserId == currentUserContext.CurrentUser.Id &&
                _.Name == request.Name)
            .AnyAsync(cancellationToken); // TODO: IDbContext<TContext>.AnyAsync<TEntity>(pred, cancellationtoken)

        if (duplicate)
        {
            response.Errors.Add(VALIDATION_DUPLICATE_TAG_NAME);
        }

        return response;
    }
}
