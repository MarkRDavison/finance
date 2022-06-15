namespace mark.davison.finance.models.Entities;

public partial class AvailableBudget : FinanceEntity
{
    public Guid CurrencyId { get; set; }
    public long Amount { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

