namespace mark.davison.finance.web.features.Dashboard.QueryAccountSummary;

public class QueryAccountTypeSummaryActionHandler : IActionHandler<QueryAccountSummaryActionRequest>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;
    private readonly IAppContextService _appContextService;

    public QueryAccountTypeSummaryActionHandler(
        IClientHttpRepository repository,
        IStateStore stateStore,
        IAppContextService appContextService
    )
    {
        _repository = repository;
        _stateStore = stateStore;
        _appContextService = appContextService;
    }

    public async Task Handle(QueryAccountSummaryActionRequest action, CancellationToken cancellationToken)
    {
        var response = await _repository.Get<AccountDashboardSummaryQueryResponse, AccountDashboardSummaryQueryRequest>(
            new AccountDashboardSummaryQueryRequest
            {
                AccountTypeId = action.AccountTypeId,
                RangeStart = _appContextService.RangeStart,
                RangeEnd = _appContextService.RangeEnd
            },
            cancellationToken);

        if (response.Success)
        {
            var existing = _stateStore.GetState<DashboardState>();
            foreach (var item in response.AccountNames)
            {
                existing.Instance.AccountNames[item.Key] = item.Value;
            }
            foreach (var item in response.TransactionData)
            {
                existing.Instance.TransactionData[item.Key] = item.Value;
            }

            _stateStore.SetState(new DashboardState(existing.Instance.AccountNames, existing.Instance.TransactionData));
        }
    }
}
