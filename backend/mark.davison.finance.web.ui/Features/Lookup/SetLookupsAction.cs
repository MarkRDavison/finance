namespace mark.davison.finance.web.ui.Features.Lookup;

public partial class LookupState
{
    public class SetLookupsAction : IAction
    {
        public List<BankDto> Banks { get; set; } = new();
        public List<AccountTypeDto> AccountTypes { get; set; } = new();
        public List<CurrencyDto> Currencies { get; set; } = new();
        public List<TransactionTypeDto> TransactionTypes { get; set; } = new();
    }
}