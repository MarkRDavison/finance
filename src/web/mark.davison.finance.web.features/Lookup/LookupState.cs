namespace mark.davison.finance.web.features.Lookup;

public class LookupState : IState
{
    public LookupState() : this(
        Enumerable.Empty<AccountTypeDto>(),
        Enumerable.Empty<CurrencyDto>(),
        Enumerable.Empty<TransactionTypeDto>())
    {
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


    public IEnumerable<AccountTypeDto> AccountTypes { get; private set; }

    public IEnumerable<CurrencyDto> Currencies { get; private set; }

    public IEnumerable<TransactionTypeDto> TransactionTypes { get; private set; }

    public void Initialise()
    {
        AccountTypes = Enumerable.Empty<AccountTypeDto>();
        Currencies = Enumerable.Empty<CurrencyDto>();
        TransactionTypes = Enumerable.Empty<TransactionTypeDto>();
    }
}
