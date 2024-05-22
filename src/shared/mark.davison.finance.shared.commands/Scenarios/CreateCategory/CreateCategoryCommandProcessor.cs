namespace mark.davison.finance.shared.commands.Scenarios.CreateCategory;

public class CreateCategoryCommandProcessor : ICommandProcessor<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public CreateCategoryCommandProcessor(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateCategoryCommandResponse> ProcessAsync(CreateCategoryCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateCategoryCommandResponse();

        var category = new Category
        {
            Id = request.Id,
            Name = request.Name,
            UserId = currentUserContext.CurrentUser.Id
        };


        var createdCategory = await _dbContext.UpsertEntityAsync(
            category,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (createdCategory == null)
        {
            response.Errors.Add("DB_CREATE_ERROR");// TODO: Common error codes???
        }

        return response;
    }
}
