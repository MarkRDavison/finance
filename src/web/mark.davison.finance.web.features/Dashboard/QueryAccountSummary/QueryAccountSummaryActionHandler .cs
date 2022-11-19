namespace mark.davison.finance.web.features.Dashboard.QueryAccountSummary;

public class QueryAccountTypeSummaryActionHandler : IActionHandler<QueryAccountSummaryActionRequest>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public QueryAccountTypeSummaryActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task Handle(QueryAccountSummaryActionRequest action, CancellationToken cancellationToken)
    {
        var response = await _repository.Get<AccountDashboardSummaryQueryResponse, AccountDashboardSummaryQueryRequest>(
            new AccountDashboardSummaryQueryRequest
            {
                AccountTypeId = action.AccountTypeId
            },
            cancellationToken);

        _stateStore.SetState(new DashboardState(response.AccountNames, response.TransactionData));
    }
}
