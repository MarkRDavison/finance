namespace mark.davison.finance.web.features.Store.DashboardUseCase;

public static class DashboardReducers
{
    [ReducerMethod]
    public static DashboardState FetchDashboardSummaryActionResponse(DashboardState state, FetchDashboardSummaryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new DashboardState(response.Value.TransactionData);
        }

        return state;
    }
}
