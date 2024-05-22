namespace mark.davison.finance.shared.commands.Scenarios.CreateTag;

public sealed class CreateTagCommandHandler : ValidateAndProcessCommandHandler<CreateTagCommandRequest, CreateTagCommandResponse>
{
    public CreateTagCommandHandler(
        ICommandProcessor<CreateTagCommandRequest, CreateTagCommandResponse> processor,
        ICommandValidator<CreateTagCommandRequest, CreateTagCommandResponse> validator
    ) : base(
        processor,
        validator)
    {
    }
}
