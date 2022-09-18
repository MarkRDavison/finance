namespace mark.davison.finance.models.Entities;
public partial class Account : FinanceEntity
{
    public static Guid OpeningBalance = new Guid("0D88BF03-BA3C-4083-9955-80BAF27CB657");
    public static Guid Reconciliation = new Guid("F1B34475-29C9-4379-A26B-2197230A14FD");

    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public long? VirtualBalance { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid AccountTypeId { get; set; }
    public Guid CurrencyId { get; set; }

    public virtual AccountType? AccountType { get; set; }
    public virtual Currency? Currency { get; set; }
}
