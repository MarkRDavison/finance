using mark.davison.common.client.abstractions.CQRS;
using mark.davison.finance.accounting.rules;
using mark.davison.finance.web.components.CommonCandidates.Form;
using mark.davison.finance.web.features.Account.Add;
using mark.davison.finance.web.features.Account.Create;

namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount;

public class EditAccountModalViewModel : IModalViewModel<EditAccountFormViewModel, EditAccountForm>
{
    private readonly ICQRSDispatcher _dispatcher;

    public EditAccountModalViewModel(
        ICQRSDispatcher dispatcher
    )
    {
        _dispatcher = dispatcher;
    }

    public EditAccountFormViewModel FormViewModel { get; set; } = new();

    public async Task<bool> Primary(EditAccountFormViewModel formViewModel)
    {
        if (!FormViewModel.Valid)
        {
            return false;
        }

        var currency = FormViewModel.LookupState.Instance.Currencies.First(_ => _.Id == FormViewModel.CurrencyId);
        var accountType = FormViewModel.LookupState.Instance.AccountTypes.First(_ => _.Id == FormViewModel.AccountTypeId);

        bool openingBalanceSpecified = FormViewModel.OpeningBalance != default;

        var request = new CreateAccountCommandRequest
        {
            Id = FormViewModel.Id,
            Name = FormViewModel.Name,
            AccountNumber = FormViewModel.AccountNumber,
            VirtualBalance = CurrencyRules.ToPersisted(FormViewModel.VirtualBalance ?? 0),
            AccountTypeId = FormViewModel.AccountTypeId ?? Guid.Empty,
            CurrencyId = FormViewModel.CurrencyId ?? Guid.Empty,
            OpeningBalance = openingBalanceSpecified ? CurrencyRules.ToPersisted(FormViewModel.OpeningBalance ?? 0) : null,
            OpeningBalanceDate = (openingBalanceSpecified && FormViewModel.OpeningBalanceDate != null) ? DateOnly.FromDateTime(FormViewModel.OpeningBalanceDate.Value) : null,
        };

        var response = await _dispatcher.Dispatch<CreateAccountCommandRequest, CreateAccountCommandResponse>(request, CancellationToken.None);

        if (response.Success && response.ItemId != Guid.Empty)
        {
            await _dispatcher.Dispatch(new UpdateAccountListItemsAction(new List<AccountListItemDto>
            {
                new AccountListItemDto {
                    Id = response.ItemId,
                    Name = FormViewModel.Name,
                    AccountNumber = FormViewModel.AccountNumber,
                    AccountType = accountType.Type,
                    Active = true,
                    BalanceDifference = 0, // TODO: Understand this
                    CurrentBalance = 0, // TODO: Understand this
                    CurrencyId = currency.Id,
                    LastModified = DateTime.UtcNow,
                    OpeningBalance = request.OpeningBalance,
                    OpeningBalanceDate = request.OpeningBalanceDate,
                }
            }), CancellationToken.None); ;

            return true;
        }

        return false;
    }
}
