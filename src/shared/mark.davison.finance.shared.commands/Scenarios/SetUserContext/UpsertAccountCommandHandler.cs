namespace mark.davison.finance.shared.commands.Scenarios.SetUserContext;

public sealed class SetUserContextCommandHandler : ValidateAndProcessCommandHandler<SetUserContextCommandRequest, SetUserContextCommandResponse>
{
    public SetUserContextCommandHandler(ICommandProcessor<SetUserContextCommandRequest, SetUserContextCommandResponse> processor) : base(processor)
    {
    }
}
