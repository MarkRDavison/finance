using mark.davison.finance.web.features.Dashboard;
using mark.davison.finance.web.features.Dashboard.QueryAccountSummary;

namespace mark.davison.finance.web.features.StateHelpers;

public class StateHelper : IStateHelper
{
    private class StateHelperDisposable : IDisposable
    {
        private bool _disposedValue;
        private readonly StateHelper _stateHelper;

        public StateHelperDisposable(StateHelper stateHelper)
        {
            _stateHelper = stateHelper;
            _stateHelper._force = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _stateHelper._force = false;
                    _disposedValue = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

    private readonly ICQRSDispatcher _dispatcher;
    private readonly IStateStore _stateStore;

    internal bool _force;

    public StateHelper(
        ICQRSDispatcher dispatcher,
        IStateStore stateStore
    )
    {
        _dispatcher = dispatcher;
        _stateStore = stateStore;
    }

    public IDisposable Force() => new StateHelperDisposable(this);

    private bool RequiresRefetch(DateTime stateLastModified, TimeSpan interval)
    {
        return _force || DateTime.Now - stateLastModified > interval;
    }

    public async Task FetchAccountList(bool showActive)
    {
        var state = _stateStore.GetState<AccountListState>();
        if (RequiresRefetch(state.Instance.LastModified, DefaultRefetchTimeSpan))
        {
            await _dispatcher.Dispatch(new FetchAccountListAction(false), CancellationToken.None);
        }
    }

    public async Task FetchCategoryList()
    {
        var state = _stateStore.GetState<CategoryListState>();
        if (RequiresRefetch(state.Instance.LastModified, DefaultRefetchTimeSpan))
        {
            await _dispatcher.Dispatch(new FetchCategoryListAction(), CancellationToken.None);
        }
    }

    public async Task FetchAccountInformation(Guid accountId)
    {
        await _dispatcher.Dispatch(new TransactionQueryByAccountAction() { AccountId = accountId }, CancellationToken.None);
    }

    public async Task FetchAccountTypeDashboardSummaryData(params Guid[] accountTypeIds)
    {
        var state = _stateStore.GetState<DashboardState>();
        if (RequiresRefetch(state.Instance.LastModified, DefaultRefetchTimeSpan))
        {
            await Task.WhenAll(accountTypeIds.Select(
                _ => _dispatcher.Dispatch(
                    new QueryAccountSummaryActionRequest { AccountTypeId = _ },
                    CancellationToken.None)));
        }
    }

    public TimeSpan DefaultRefetchTimeSpan => TimeSpan.FromMinutes(1);

}
