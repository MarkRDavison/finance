namespace mark.davison.finance.bff.commands.Scenarios.CreateLocation;

[PostRequest(Path = "create-account")]
public class CreateAccountRequest : ICommand<CreateAccountRequest, CreateAccountResponse>
{
    public CreateAccountDto CreateAccountDto { get; set; } = new();
}