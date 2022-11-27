namespace mark.davison.finance.models.Models;

public class AccountSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public Guid AccountTypeId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastActivity { get; set; }
    public long Balance { get; set; }
    public long BalanceDifference { get; set; }
    public long? VirtualBalance { get; set; }
    public long? OpeningBalance { get; set; }
    public DateOnly? OpeningBalanceDate { get; set; }
    public Guid CurrencyId { get; set; }
}

