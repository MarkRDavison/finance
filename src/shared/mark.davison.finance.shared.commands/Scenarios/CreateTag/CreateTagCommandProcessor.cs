namespace mark.davison.finance.shared.commands.Scenarios.CreateTag;

public sealed class CreateTagCommandProcessor : ICommandProcessor<CreateTagCommandRequest, CreateTagCommandResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public CreateTagCommandProcessor(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateTagCommandResponse> ProcessAsync(CreateTagCommandRequest command, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTagCommandResponse();
        // TODO: Replace with validator/processor pattern
        var tag = new Tag
        {
            Id = command.Id,
            Name = command.Name,
            MinDate = command.MinDate,
            MaxDate = command.MaxDate,
            UserId = currentUserContext.CurrentUser.Id
        };

        await _dbContext.UpsertEntityAsync(tag, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

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
