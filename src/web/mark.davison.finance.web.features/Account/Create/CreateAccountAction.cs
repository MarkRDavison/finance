namespace mark.davison.finance.web.features.Account.Create;

public class CreateAccountAction : ICommand<CreateAccountAction, CreateAccountCommandResult>
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