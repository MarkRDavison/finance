namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount.Common;

public class EditAccountFormSubmission : IFormSubmission<EditAccountFormViewModel>
{
    private readonly IStoreHelper _storeHelper;
    private readonly IState<StartupState> _startupState;

    public EditAccountFormSubmission(
        IStoreHelper storeHelper,
        IState<StartupState> startupState)
    {
        _storeHelper = storeHelper;
        _startupState = startupState;
    }

    public async Task<Response> Primary(EditAccountFormViewModel formViewModel)
    {
        if (formViewModel.Id == Guid.Empty)
        {
            formViewModel.Id = Guid.NewGuid();
        }

        var currency = _startupState.Value.Currencies.First(_ => _.Id == formViewModel.CurrencyId);
        var accountType = _startupState.Value.AccountTypes.First(_ => _.Id == formViewModel.AccountTypeId);

        bool openingBalanceSpecified = formViewModel.OpeningBalance != default;

        var action = new CreateAccountAction
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                Id = formViewModel.Id,
                Name = formViewModel.Name,
                AccountNumber = formViewModel.AccountNumber,
                VirtualBalance = CurrencyRules.ToPersisted(formViewModel.VirtualBalance ?? 0),
                AccountTypeId = formViewModel.AccountTypeId ?? Guid.Empty,
                CurrencyId = formViewModel.CurrencyId ?? Guid.Empty,
                OpeningBalance = openingBalanceSpecified ? CurrencyRules.ToPersisted(formViewModel.OpeningBalance ?? 0) : null,
                OpeningBalanceDate = openingBalanceSpecified && formViewModel.OpeningBalanceDate != null ? DateOnly.FromDateTime(formViewModel.OpeningBalanceDate.Value) : null,
            }
        };

        var actionResponse = await _storeHelper.DispatchAndWaitForResponse<CreateAccountAction, CreateAccountActionResponse>(action);

        return actionResponse;
    }
}
