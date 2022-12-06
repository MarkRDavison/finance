namespace mark.davison.finance.models.dtos.Queries.AccountDashboardSummaryQuery;

[GetRequest(Path = "account-dashboard-summary-query")]
public class AccountDashboardSummaryQueryRequest : IQuery<AccountDashboardSummaryQueryRequest, AccountDashboardSummaryQueryResponse>
{
    public Guid AccountTypeId { get; set; }
    public DateOnly RangeStart { get; set; }
    public DateOnly RangeEnd { get; set; }
}
