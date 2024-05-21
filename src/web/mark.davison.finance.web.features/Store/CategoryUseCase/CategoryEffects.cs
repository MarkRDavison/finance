namespace mark.davison.finance.web.features.Store.CategoryUseCase;

public sealed class CategoryEffects
{
    private readonly IClientHttpRepository _repository;

    public CategoryEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchCategoriesActionAsync(FetchCategoriesAction action, IDispatcher dispatcher)
    {
        // TODO: Rename query req/res
        var queryResponse = await _repository.Get<CategoryListQueryResponse, CategoryListQueryRequest>(CancellationToken.None);

        var actionResponse = FetchCategoriesActionResponse.From(queryResponse);
        actionResponse.ActionId = action.ActionId;
        actionResponse.Value = queryResponse.Value;

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleCreateCategoryActionAsync(CreateCategoryAction action, IDispatcher dispatcher)
    {
        var commandRequest = new CreateCategoryCommandRequest
        {
            Id = action.Id,
            Name = action.Name
        };

        var commandResponse = await _repository.Post<CreateCategoryCommandResponse, CreateCategoryCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = CreateCategoryActionResponse.From(commandResponse);
        actionResponse.ActionId = action.ActionId;
        actionResponse.Value = commandResponse.Value;

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
