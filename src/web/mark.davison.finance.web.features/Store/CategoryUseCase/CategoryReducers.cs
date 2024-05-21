namespace mark.davison.finance.web.features.Store.CategoryUseCase;

public static class CategoryReducers
{

    [ReducerMethod]
    public static CategoryState FetchCategoriesActionResponse(CategoryState state, FetchCategoriesActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new CategoryState(response.Value);
        }

        return state;
    }

    [ReducerMethod]
    public static CategoryState CreateCategoryActionResponse(CategoryState state, CreateCategoryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new CategoryState([.. state.Categories, response.Value]);
        }

        return state;
    }
}
