namespace mark.davison.finance.web.features.Store.DashboardUseCase;

[FeatureState]
public sealed class DashboardState
{
    public DashboardState() : this(false, [])
    {

    }

    public DashboardState(
        bool loading,
        Dictionary<Guid, List<AccountDashboardTransactionData>> transactionData)
    {
        Loading = loading;
        TransactionData = transactionData;
    }

    public bool Loading { get; set; }
    public IDictionary<Guid, List<AccountDashboardTransactionData>> TransactionData { get; }

}
