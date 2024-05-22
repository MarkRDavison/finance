namespace mark.davison.finance.shared.commands.Scenarios.CreateTag;

public sealed class CreateTagCommandValidator : ICommandValidator<CreateTagCommandRequest, CreateTagCommandResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public const string VALIDATION_DUPLICATE_TAG_NAME = "VALIDATION_DUPLICATE_TAG_NAME";

    public CreateTagCommandValidator(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateTagCommandResponse> ValidateAsync(CreateTagCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTagCommandResponse { };

        var duplicate = await _dbContext.AnyAsync<Tag>(
            _ =>
                _.UserId == currentUserContext.CurrentUser.Id &&
                _.Name == request.Name,
            cancellationToken);

        if (duplicate)
        {
            response.Errors.Add(VALIDATION_DUPLICATE_TAG_NAME);
        }

        return response;
    }
}
