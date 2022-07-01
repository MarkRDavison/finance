﻿namespace mark.davison.finance.web.ui.Features.Account.Create;

public class CreateAccountCommand : ICommand<CreateAccountCommand, CreateAccountCommandResult>
{
    public string Name { get; set; } = string.Empty;
    public long? VirtualBalance { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public Guid BankId { get; set; }
    public Guid AccountTypeId { get; set; }
    public Guid CurrencyId { get; set; }
}

public class CreateAccountCommandResult
{
    public bool Success { get; set; }
    public Guid ItemId { get; set; }
}