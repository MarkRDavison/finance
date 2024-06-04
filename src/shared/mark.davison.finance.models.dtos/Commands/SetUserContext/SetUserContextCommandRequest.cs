namespace mark.davison.finance.models.dtos.Commands.SetUserContext;

[PostRequest(Path = "set-user-context-command")]
public sealed class SetUserContextCommandRequest : ICommand<SetUserContextCommandRequest, SetUserContextCommandResponse>
{
    public UserContextDto UserContext { get; set; } = new();
}
