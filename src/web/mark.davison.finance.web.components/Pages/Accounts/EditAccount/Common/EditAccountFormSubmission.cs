using mark.davison.common.client.abstractions.CQRS;
using mark.davison.finance.accounting.rules;
using mark.davison.finance.web.features.Account.Add;
using mark.davison.finance.web.features.Account.Create;

namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount.Common;

public class EditAccountFormSubmission : IEditAccountFormSubmission
{
    private readonly ICQRSDispatcher _dispatcher;

    public EditAccountFormSubmission(
        ICQRSDispatcher dispatcher
    )
    {
        _dispatcher = dispatcher;
    }

    public async Task<bool> Primary(EditAccountFormViewModel formViewModel)
    {
        if (!formViewModel.Valid)
        {
            return false;
        }

        if (formViewModel.Id == Guid.Empty)
        {
            formViewModel.Id = Guid.NewGuid();
        }

        var currency = formViewModel.LookupState.Instance.Currencies.First(_ => _.Id == formViewModel.CurrencyId);
        var accountType = formViewModel.LookupState.Instance.AccountTypes.First(_ => _.Id == formViewModel.AccountTypeId);

        bool openingBalanceSpecified = formViewModel.OpeningBalance != default;

        var request = new CreateAccountCommandRequest
        {
            Id = formViewModel.Id,
            Name = formViewModel.Name,
            AccountNumber = formViewModel.AccountNumber,
            VirtualBalance = CurrencyRules.ToPersisted(formViewModel.VirtualBalance ?? 0),
            AccountTypeId = formViewModel.AccountTypeId ?? Guid.Empty,
            CurrencyId = formViewModel.CurrencyId ?? Guid.Empty,
            OpeningBalance = openingBalanceSpecified ? CurrencyRules.ToPersisted(formViewModel.OpeningBalance ?? 0) : null,
            OpeningBalanceDate = openingBalanceSpecified && formViewModel.OpeningBalanceDate != null ? DateOnly.FromDateTime(formViewModel.OpeningBalanceDate.Value) : null,
        };

        var response = await _dispatcher.Dispatch<CreateAccountCommandRequest, CreateAccountCommandResponse>(request, CancellationToken.None);

        if (response.Success && response.ItemId != Guid.Empty)
        {
            await _dispatcher.Dispatch(new UpdateAccountListItemsAction(new List<AccountListItemDto>
            {
                new AccountListItemDto {
                    Id = response.ItemId,
                    Name = formViewModel.Name,
                    AccountNumber = formViewModel.AccountNumber,
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
