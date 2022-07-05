namespace mark.davison.finance.web.ui.Features.Lookup;

public class LookupState : IState
{
    public LookupState()
    {
        Banks = Enumerable.Empty<BankDto>();
        AccountTypes = Enumerable.Empty<AccountTypeDto>();
        Currencies = Enumerable.Empty<CurrencyDto>();
        TransactionTypes = Enumerable.Empty<TransactionTypeDto>();
    }
    public LookupState(
        IEnumerable<BankDto> banks,
        IEnumerable<AccountTypeDto> accountTypes,
        IEnumerable<CurrencyDto> currencies,
        IEnumerable<TransactionTypeDto> transactionTypes
    )
    {
        Banks = banks;
        AccountTypes = accountTypes;
        Currencies = currencies;
        TransactionTypes = transactionTypes;
    }


    public IEnumerable<BankDto> Banks { get; init; }

    public IEnumerable<AccountTypeDto> AccountTypes { get; init; }

    public IEnumerable<CurrencyDto> Currencies { get; init; }

    public IEnumerable<TransactionTypeDto> TransactionTypes { get; init; }

    public List<IDropdownItem<Guid>> BankItems => Banks.Select(_ => new DropdownItem<Guid>(_.Id, _.Name)).Cast<IDropdownItem<Guid>>().ToList();

    public List<IDropdownItem<Guid>> AccountTypeItems => AccountTypes.Select(_ => new DropdownItem<Guid>(_.Id, _.Type)).Cast<IDropdownItem<Guid>>().ToList();

    public List<IDropdownItem<Guid>> CurrencyItems => Currencies.Select(_ => new DropdownItem<Guid>(_.Id, _.Name)).Cast<IDropdownItem<Guid>>().ToList();


    public void Initialise()
    {
    }
}
