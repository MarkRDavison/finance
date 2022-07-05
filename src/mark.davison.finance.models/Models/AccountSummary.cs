namespace mark.davison.finance.models.Models;

public class AccountSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime LastActivity { get; set; }
    public long Balance { get; set; }
    public long BalanceDifference { get; set; }
}

