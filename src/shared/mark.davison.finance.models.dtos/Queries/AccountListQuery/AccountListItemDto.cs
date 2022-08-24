namespace mark.davison.finance.models.dtos.Queries.AccountListQuery;

public class AccountListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public Guid AccountTypeId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public long CurrentBalance { get; set; }
    public bool Active { get; set; }
    public DateTime LastModified { get; set; }
    public long BalanceDifference { get; set; }
    public Guid CurrencyId { get; set; }
}

