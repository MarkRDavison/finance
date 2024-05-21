namespace mark.davison.finance.web.features.Store.TagUseCase;

public static class TagReducers
{
    [ReducerMethod]
    public static TagState FetchTagsActionResponse(TagState state, FetchTagsActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new TagState(response.Value);
        }

        return state;
    }

    [ReducerMethod]
    public static TagState CreateTagActionResponse(TagState state, CreateTagActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new TagState([.. state.Tags, response.Value]);
        }

        return state;
    }
}
