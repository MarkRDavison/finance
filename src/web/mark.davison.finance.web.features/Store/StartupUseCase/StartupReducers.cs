namespace mark.davison.finance.web.features.Store.StartupUseCase;

public static class StartupReducers
{
    [ReducerMethod]
    public static StartupState FetchStartupActionResponse(StartupState state, FetchStartupActionResponse response)
    {
        Console.WriteLine("StartupReducers.FetchStartupActionResponse");
        if (response.SuccessWithValue)
        {
            return new StartupState(
                response.Value.AccountTypes,
                response.Value.Currencies,
                response.Value.TransactionTypes);
        }

        return state;
    }
}
