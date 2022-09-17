namespace mark.davison.finance.web.features.Lookup;

public class LookupState : IState
{
    public LookupState()
    {
        AccountTypes = Enumerable.Empty<AccountTypeDto>();
        Currencies = Enumerable.Empty<CurrencyDto>();
        TransactionTypes = Enumerable.Empty<TransactionTypeDto>();
    }

    public LookupState(
        IEnumerable<AccountTypeDto> accountTypes,
        IEnumerable<CurrencyDto> currencies,
        IEnumerable<TransactionTypeDto> transactionTypes
    )
    {
        AccountTypes = accountTypes;
        Currencies = currencies;
        TransactionTypes = transactionTypes;
    }


    public IEnumerable<AccountTypeDto> AccountTypes { get; init; }

    public IEnumerable<CurrencyDto> Currencies { get; init; }

    public IEnumerable<TransactionTypeDto> TransactionTypes { get; init; }

    public void Initialise()
    {
    }
}
