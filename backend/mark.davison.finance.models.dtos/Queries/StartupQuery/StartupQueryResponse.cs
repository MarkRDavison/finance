namespace mark.davison.finance.models.dtos.Queries.StartupQuery;

public class StartupQueryResponse
{
    public List<BankDto> Banks { get; set; } = new();
    public List<AccountTypeDto> AccountTypes { get; set; } = new();
    public List<CurrencyDto> Currencies { get; set; } = new();
    public List<TransactionTypeDto> TransactionTypes { get; set; } = new();
}

