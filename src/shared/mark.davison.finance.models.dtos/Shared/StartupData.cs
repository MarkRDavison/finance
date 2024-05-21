namespace mark.davison.finance.models.dtos.Shared;
public sealed class StartupData
{
    public List<AccountTypeDto> AccountTypes { get; set; } = new();
    public List<CurrencyDto> Currencies { get; set; } = new();
    public List<TransactionTypeDto> TransactionTypes { get; set; } = new();
}
