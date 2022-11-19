namespace mark.davison.finance.web.features.Dashboard.QueryAccountSummary;

public class QueryAccountSummaryActionRequest : IAction<QueryAccountSummaryActionRequest>
{
    public Guid AccountTypeId { get; set; }
}
