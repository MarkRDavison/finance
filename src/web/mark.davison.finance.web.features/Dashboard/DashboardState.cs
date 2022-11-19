namespace mark.davison.finance.web.features.Dashboard;

public class DashboardState : IState
{
    public DashboardState() : this(new(), new())
    {

    }
    public DashboardState(
        Dictionary<Guid, string> accountNames,
        Dictionary<Guid, List<AccountDashboardTransactionData>> transactionData)
    {
        LastModified = DateTime.UtcNow;
        AccountNames = accountNames;
        TransactionData = transactionData;
    }

    public Dictionary<Guid, string> AccountNames { get; init; }
    public Dictionary<Guid, List<AccountDashboardTransactionData>> TransactionData { get; init; }
    public DateTime LastModified { get; private set; }

    public void Initialise()
    {
        LastModified = default;
    }
}
