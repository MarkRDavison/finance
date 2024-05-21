namespace mark.davison.finance.web.features.Store.DashboardUseCase;

[FeatureState]
public sealed class DashboardState
{
    public DashboardState() : this([], [])
    {

    }

    public DashboardState(
        Dictionary<Guid, string> accountNames,
        Dictionary<Guid, List<AccountDashboardTransactionData>> transactionData)
    {
        AccountNames = accountNames;
        TransactionData = transactionData;
    }

    public IDictionary<Guid, string> AccountNames { get; }
    public IDictionary<Guid, List<AccountDashboardTransactionData>> TransactionData { get; }

}
