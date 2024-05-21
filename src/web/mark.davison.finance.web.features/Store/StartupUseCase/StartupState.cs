namespace mark.davison.finance.web.features.Store.StartupUseCase;

[FeatureState]
public class StartupState
{
    public StartupState() : this([], [], [])
    {
    }

    public StartupState(
        IEnumerable<AccountTypeDto> accountTypes,
        IEnumerable<CurrencyDto> currencies,
        IEnumerable<TransactionTypeDto> transactionTypes
    )
    {
        AccountTypes = new(accountTypes.ToList());
        Currencies = new(currencies.ToList());
        TransactionTypes = new(transactionTypes.ToList());
    }


    public ReadOnlyCollection<AccountTypeDto> AccountTypes { get; }

    public ReadOnlyCollection<CurrencyDto> Currencies { get; }

    public ReadOnlyCollection<TransactionTypeDto> TransactionTypes { get; }
}
