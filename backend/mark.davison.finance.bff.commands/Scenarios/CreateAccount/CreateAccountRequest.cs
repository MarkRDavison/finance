namespace mark.davison.finance.bff.commands.Scenarios.CreateLocation;

[PostRequest(Path = "create-account")]
public class CreateAccountRequest : ICommand<CreateAccountRequest, CreateAccountResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? VirtualBalance { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public Guid BankId { get; set; }
    public Guid AccountTypeId { get; set; }
    public Guid CurrencyId { get; set; }
}