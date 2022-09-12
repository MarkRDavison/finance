namespace mark.davison.finance.models.dtos.Commands.CreateAccount;

[PostRequest(Path = "create-account")]
public class CreateAccountRequest : ICommand<CreateAccountRequest, CreateAccountResponse>
{
    public CreateAccountDto CreateAccountDto { get; set; } = new();
}