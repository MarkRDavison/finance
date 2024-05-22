namespace mark.davison.finance.web.features.Store.TagUseCase;

public sealed class TagEffects
{
    private readonly IClientHttpRepository _repository;

    public TagEffects(IClientHttpRepository repository)
    {
        _repository = repository;
    }

    [EffectMethod]
    public async Task HandleFetchTagsActionAsync(FetchTagsAction action, IDispatcher dispatcher)
    {
        // TODO: Rename query req/res
        var queryResponse = await _repository.Get<TagListQueryResponse, TagListQueryRequest>(CancellationToken.None);

        var actionResponse = new FetchTagsActionResponse
        {
            ActionId = action.ActionId,
            Errors = queryResponse.Errors,
            Warnings = queryResponse.Warnings,
            Value = queryResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }

    [EffectMethod]
    public async Task HandleCreateTagActionAsync(CreateTagAction action, IDispatcher dispatcher)
    {
        var commandRequest = new CreateTagCommandRequest
        {
            Id = action.Id,
            Name = action.Name
        };

        var commandResponse = await _repository.Post<CreateTagCommandResponse, CreateTagCommandRequest>(commandRequest, CancellationToken.None);

        var actionResponse = new CreateTagActionResponse
        {
            ActionId = action.ActionId,
            Errors = commandResponse.Errors,
            Warnings = commandResponse.Warnings,
            Value = commandResponse.Value
        };

        // TODO: Framework to dispatch general ***something went wrong***

        dispatcher.Dispatch(actionResponse);
    }
}
