namespace mark.davison.finance.models.Entities;

public partial class Bill : FinanceEntity
{
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public long AmountMin { get; set; }
    public long AmountMax { get; set; }
    public DateOnly Date { get; set; }
    public string RepeatFrequency { get; set; } = string.Empty; // TODO: External table?
    public bool AutoMatch { get; set; }
    public Guid CurrencyId { get; set; }
    public int Order { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly? ExtensionDate { get; set; }

}

