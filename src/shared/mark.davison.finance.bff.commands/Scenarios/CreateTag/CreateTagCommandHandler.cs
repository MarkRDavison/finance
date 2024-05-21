namespace mark.davison.finance.bff.commands.Scenarios.CreateTag;

public class CreateTagCommandHandler : ICommandHandler<CreateTagCommandRequest, CreateTagCommandResponse>
{
    private readonly IFinanceDbContext _dbContext;
    private readonly ICreateTagCommandValidator _createTagCommandValidator;

    public CreateTagCommandHandler(
        IFinanceDbContext dbContext,
        ICreateTagCommandValidator createTagCommandValidator
    )
    {
        _dbContext = dbContext;
        _createTagCommandValidator = createTagCommandValidator;
    }

    public async Task<CreateTagCommandResponse> Handle(CreateTagCommandRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _createTagCommandValidator.Validate(command, currentUserContext, cancellationToken);

        if (!response.Success)
        {
            return response;
        }

        // TODO: Replace with validator/processor pattern
        var category = new Tag
        {
            Id = command.Id,
            Name = command.Name,
            MinDate = command.MinDate,
            MaxDate = command.MaxDate,
            UserId = currentUserContext.CurrentUser.Id
        };

        var tag = await _dbContext.UpsertEntityAsync(category, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (tag == null)
        {
            response.Errors.Add("DB_UPSERT_ERROR"); // TODO: Standard db errors
            return response;
        }

        response.Value = new TagDto // TODO: Helper
        {
            Id = tag.Id,
            Name = tag.Name,
            MinDate = tag.MinDate,
            MaxDate = tag.MaxDate
        };

        return response;
    }
}
