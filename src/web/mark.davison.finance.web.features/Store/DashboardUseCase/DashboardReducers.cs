namespace mark.davison.finance.web.features.Store.DashboardUseCase;

public static class DashboardReducers
{
    [ReducerMethod]
    public static DashboardState FetchDashboardSummaryAction(DashboardState state, FetchDashboardSummaryAction action)
    {
        return new DashboardState(true, []);
    }
    [ReducerMethod]
    public static DashboardState FetchDashboardSummaryActionResponse(DashboardState state, FetchDashboardSummaryActionResponse response)
    {
        if (response.SuccessWithValue)
        {
            return new DashboardState(false, response.Value.TransactionData);
        }

        return new DashboardState(false, []);
    }
}
