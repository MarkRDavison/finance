namespace mark.davison.finance.web.ui.Features.Lookup;

public partial class LookupState : State<LookupState>
{
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

    public override void Initialize()
    {
    }
}
