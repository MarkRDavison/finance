using mark.davison.finance.web.features.Store.AccountUseCase;
using mark.davison.finance.web.features.Store.CategoryUseCase;
using mark.davison.finance.web.features.Store.DashboardUseCase;
using mark.davison.finance.web.features.Store.TagUseCase;
using mark.davison.finance.web.features.Store.TransactionUseCase;

namespace mark.davison.finance.web.features.StateHelpers;

public sealed class StateHelper : IStateHelper
{
    private readonly IStoreHelper _storeHelper;
    private bool _forced;

    private class StateHelperDisposable : IDisposable
    {
        private readonly StateHelper _stateHelper;

        public StateHelperDisposable(StateHelper stateHelper)
        {
            _stateHelper = stateHelper;
            _stateHelper._forced = true;
        }

        public void Dispose()
        {
            _stateHelper._forced = false;
        }
    }

    public StateHelper(IStoreHelper storeHelper)
    {
        _storeHelper = storeHelper;
    }

    public TimeSpan DefaultRefetchTimeSpan => TimeSpan.FromSeconds(30);

    public async Task FetchAccountInformation(Guid accountId)
    {
        var action = new FetchTransactionsByAccount { AccountId = accountId };
        await _storeHelper.DispatchAndWaitForResponse<FetchTransactionsByAccount, FetchTransactionsByAccountResponse>(action);
    }

    public async Task FetchAccountList(bool showActive)
    {
        await _storeHelper.DispatchAndWaitForResponse<FetchAccountsAction, FetchAccountsActionResponse>(new FetchAccountsAction());
    }

    public async Task FetchAccountTypeDashboardSummaryData(params Guid[] accountTypeIds)
    {
        await Task.WhenAll(accountTypeIds.Select(
            _ => _storeHelper.DispatchAndWaitForResponse<FetchDashboardSummaryAction, FetchDashboardSummaryActionResponse>(
                    new FetchDashboardSummaryAction
                    {
                        AccountTypeId = _
                    })));
    }

    public async Task FetchCategoryList()
    {
        await _storeHelper.DispatchAndWaitForResponse<FetchCategoriesAction, FetchCategoriesActionResponse>(new FetchCategoriesAction());
    }

    public async Task FetchTagList()
    {
        await _storeHelper.DispatchAndWaitForResponse<FetchTagsAction, FetchTagsActionResponse>(new FetchTagsAction());
    }

    public async Task FetchTransactionInformation(Guid transactionGroupId)
    {
        var action = new FetchTransactionsByGroup { TransactionGroupId = transactionGroupId };
        await _storeHelper.DispatchAndWaitForResponse<FetchTransactionsByGroup, FetchTransactionsByGroupResponse>(action);
    }

    public IDisposable Force() => new StateHelperDisposable(this);
}
