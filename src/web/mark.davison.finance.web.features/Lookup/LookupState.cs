namespace mark.davison.finance.web.features.Lookup;

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

    public void Initialise()
    {
    }
}
