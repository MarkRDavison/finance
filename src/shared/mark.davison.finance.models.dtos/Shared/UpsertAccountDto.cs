﻿namespace mark.davison.finance.models.dtos.Shared;

public class UpsertAccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? VirtualBalance { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public Guid AccountTypeId { get; set; }
    public Guid CurrencyId { get; set; }
    public long? OpeningBalance { get; set; }
    public DateOnly? OpeningBalanceDate { get; set; }
}
