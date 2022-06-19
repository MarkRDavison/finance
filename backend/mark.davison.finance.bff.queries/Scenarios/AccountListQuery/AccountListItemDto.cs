﻿namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

public class AccountListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public long CurrentBalance { get; set; }
    public bool Active { get; set; }
    public DateTime LastModified { get; set; }
    public long BalanceDifference { get; set; }
}

