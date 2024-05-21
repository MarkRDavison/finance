namespace mark.davison.finance.web.features.Store.DashboardUseCase;

public sealed class FetchDashboardSummaryAction : BaseAction
{
    public Guid AccountTypeId { get; set; }
}

public sealed class FetchDashboardSummaryActionResponse : BaseActionResponse<DashboardSummaryData>
{

}