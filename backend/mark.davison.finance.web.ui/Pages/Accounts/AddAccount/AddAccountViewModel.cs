using mark.davison.finance.web.ui.Features.Account.Add;
using mark.davison.finance.web.ui.Features.Account.Create;

namespace mark.davison.finance.web.ui.Pages.Accounts.AddAccount;

public partial class AddAccountViewModel : ObservableObject
{

    private readonly ICQRSDispatcher _dispatcher;

    public AddAccountViewModel(
        ICQRSDispatcher dispatcher
    )
    {
        _dispatcher = dispatcher;
    }

    [ObservableProperty]
    private AddAccountFormViewModel _addAccountFormViewModel = new();

    public async Task<bool> OnSave()
    {
        if (!AddAccountFormViewModel.Valid)
        {
            return false;
        }

        var currency = AddAccountFormViewModel.LookupState.Instance.Currencies.First(_ => _.Id.ToString() == AddAccountFormViewModel.CurrencyId);
        var accountType = AddAccountFormViewModel.LookupState.Instance.AccountTypes.First(_ => _.Id.ToString() == AddAccountFormViewModel.AccountTypeId);

        var response = await _dispatcher.Dispatch<CreateAccountCommand, CreateAccountCommandResult>(new CreateAccountCommand
        {
            Name = AddAccountFormViewModel.Name,
            AccountNumber = AddAccountFormViewModel.AccountNumber,
            VirtualBalance = decimal.ToInt64(AddAccountFormViewModel.VirtualBalance * (decimal)Math.Pow(10, currency.DecimalPlaces)),
            AccountTypeId = Guid.Parse(AddAccountFormViewModel.AccountTypeId), // TODO: Make dropdown list bind to T, so Guid can be used without parsing
            BankId = Guid.Parse(AddAccountFormViewModel.BankId),
            CurrencyId = Guid.Parse(AddAccountFormViewModel.CurrencyId)
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
