namespace mark.davison.finance.models.dtos.Queries.AccountDashboardSummaryQuery;

public class AccountDashboardSummaryQueryResponse
{
    public bool Success { get; set; }
    public Dictionary<Guid, string> AccountNames { get; set; } = new();
    public Dictionary<Guid, List<AccountDashboardTransactionData>> TransactionData { get; set; } = new();
}
