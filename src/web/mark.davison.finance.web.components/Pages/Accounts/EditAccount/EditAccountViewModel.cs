using mark.davison.common.client.abstractions.CQRS;
using mark.davison.finance.accounting.rules;
using mark.davison.finance.models.dtos.Queries.AccountListQuery;
using mark.davison.finance.web.features.Account.Add;
using mark.davison.finance.web.features.Account.Create;

namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount;

public class EditAccountViewModel
{

    private readonly ICQRSDispatcher _dispatcher;

    public EditAccountViewModel(
        ICQRSDispatcher dispatcher
    )
    {
        _dispatcher = dispatcher;
    }

    public EditAccountFormViewModel EditAccountFormViewModel { get; set; } = new();

    public async Task<bool> OnSave()
    {
        if (!EditAccountFormViewModel.Valid)
        {
            return false;
        }

        var currency = EditAccountFormViewModel.LookupState.Instance.Currencies.First(_ => _.Id == EditAccountFormViewModel.CurrencyId);
        var accountType = EditAccountFormViewModel.LookupState.Instance.AccountTypes.First(_ => _.Id == EditAccountFormViewModel.AccountTypeId);

        bool openingBalanceSpecified = EditAccountFormViewModel.OpeningBalance != default;

        var request = new CreateAccountCommandRequest
        {
            Id = EditAccountFormViewModel.Id,
            Name = EditAccountFormViewModel.Name,
            AccountNumber = EditAccountFormViewModel.AccountNumber,
            VirtualBalance = CurrencyRules.ToPersisted(EditAccountFormViewModel.VirtualBalance),
            AccountTypeId = EditAccountFormViewModel.AccountTypeId ?? Guid.Empty,
            CurrencyId = EditAccountFormViewModel.CurrencyId ?? Guid.Empty,
            OpeningBalance = openingBalanceSpecified ? CurrencyRules.ToPersisted(EditAccountFormViewModel.OpeningBalance) : null,
            OpeningBalanceDate = (openingBalanceSpecified && EditAccountFormViewModel.OpeningBalanceDate != null) ? DateOnly.FromDateTime(EditAccountFormViewModel.OpeningBalanceDate.Value) : null,
        };
        var response = await _dispatcher.Dispatch<CreateAccountCommandRequest, CreateAccountCommandResponse>(request, CancellationToken.None);

        if (response.Success && response.ItemId != Guid.Empty)
        {
            await _dispatcher.Dispatch(new UpdateAccountListItemsAction(new List<AccountListItemDto>
            {
                new AccountListItemDto {
                    Id = response.ItemId,
                    Name = EditAccountFormViewModel.Name,
                    AccountNumber = EditAccountFormViewModel.AccountNumber,
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

    public async Task OnCancel()
    {
        await Task.CompletedTask;
    }
}
