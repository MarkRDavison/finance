namespace mark.davison.finance.web.features.Store.DashboardUseCase;

[FeatureState]
public sealed class DashboardState
{
    public DashboardState() : this([])
    {

    }

    public DashboardState(
        Dictionary<Guid, List<AccountDashboardTransactionData>> transactionData)
    {
        TransactionData = transactionData;
    }

    public IDictionary<Guid, List<AccountDashboardTransactionData>> TransactionData { get; }

}
