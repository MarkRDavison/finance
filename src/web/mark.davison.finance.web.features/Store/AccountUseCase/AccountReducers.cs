namespace mark.davison.finance.web.features.Store.AccountUseCase;

public static class AccountReducers
{
    [ReducerMethod]
    public static AccountState FetchAccountsActionResponse(AccountState state, FetchAccountsActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new AccountState(response.Value);
        }

        return state;
    }

    [ReducerMethod]
    public static AccountState CreateAccountActionResponse(AccountState state, CreateAccountActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new AccountState([response.Value, .. state.Accounts.Where(_ => _.Id != response.Value.Id)]);
        }

        return state;
    }
}
