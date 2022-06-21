namespace mark.davison.finance.bff.queries.Scenarios.StartupQuery;

public class StartupQueryResponse
{
    public List<BankDto> Banks { get; set; } = new();
    public List<AccountTypeDto> AccountTypes { get; set; } = new();
    public List<CurrencyDto> Currencies { get; set; } = new();
}

