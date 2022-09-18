namespace mark.davison.finance.web.ui.Pages.Accounts.AddAccount;

public partial class AddAccountViewModel
{

    private readonly ICQRSDispatcher _dispatcher;

    public AddAccountViewModel(
        ICQRSDispatcher dispatcher
    )
    {
        _dispatcher = dispatcher;
    }

    public AddAccountFormViewModel AddAccountFormViewModel { get; set; } = new();

    public async Task<bool> OnSave()
    {
        if (!AddAccountFormViewModel.Valid)
        {
            return false;
        }

        var currency = AddAccountFormViewModel.LookupState.Instance.Currencies.First(_ => _.Id == AddAccountFormViewModel.CurrencyId);
        var accountType = AddAccountFormViewModel.LookupState.Instance.AccountTypes.First(_ => _.Id == AddAccountFormViewModel.AccountTypeId);

        bool openingBalanceSpecified = AddAccountFormViewModel.OpeningBalance != default;

        var response = await _dispatcher.Dispatch<CreateAccountCommandRequest, CreateAccountCommandResponse>(new CreateAccountCommandRequest
        {
            Name = AddAccountFormViewModel.Name,
            AccountNumber = AddAccountFormViewModel.AccountNumber,
            VirtualBalance = CurrencyRules.ToPersisted(AddAccountFormViewModel.VirtualBalance),
            AccountTypeId = AddAccountFormViewModel.AccountTypeId,
            CurrencyId = AddAccountFormViewModel.CurrencyId,
            OpeningBalance = openingBalanceSpecified ? CurrencyRules.ToPersisted(AddAccountFormViewModel.OpeningBalance) : null,
            OpeningBalanceDate = openingBalanceSpecified ? AddAccountFormViewModel.OpeningBalanceDate : null,
        }, CancellationToken.None);

        if (response.Success && response.ItemId != Guid.Empty)
        {
            await _dispatcher.Dispatch(new UpdateAccountListItemsAction(new List<AccountListItemDto>
            {
                new AccountListItemDto {
                    Id = response.ItemId,
                    Name = AddAccountFormViewModel.Name,
                    AccountNumber = AddAccountFormViewModel.AccountNumber,
                    AccountType = accountType.Type,
                    Active = true,
                    BalanceDifference = 0, // TODO: Understand this
                    CurrentBalance = 0, // TODO: Understand this
                    CurrencyId = currency.Id,
                    LastModified = DateTime.UtcNow
                }
            }), CancellationToken.None); ;

            return true;
        }

        return false;
    }

    public async Task OnCancel()
    {
        await Task.CompletedTask;
    }
}
