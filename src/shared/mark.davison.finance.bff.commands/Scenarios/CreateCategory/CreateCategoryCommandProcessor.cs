namespace mark.davison.finance.bff.commands.Scenarios.CreateCategory;

public class CreateCategoryCommandProcessor : ICommandProcessor<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
{
    private readonly IRepository _repository;
    public CreateCategoryCommandProcessor(IRepository repository)
    {
        _repository = repository;
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

        await using (_repository.BeginTransaction())
        {
            var createdCategory = await _repository.UpsertEntityAsync(
                category,
                cancellationToken);

            if (createdCategory == null)
            {
                response.Errors.Add("DB_CREATE_ERROR");// TODO: Common error codes???
            }
        }

        return response;
    }
}
