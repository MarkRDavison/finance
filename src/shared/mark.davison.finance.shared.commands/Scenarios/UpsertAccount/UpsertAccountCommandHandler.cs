namespace mark.davison.finance.shared.commands.Scenarios.CreateAccount;

public class UpsertAccountCommandHandler : ValidateAndProcessCommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>
{
    public UpsertAccountCommandHandler(
        ICommandProcessor<UpsertAccountCommandRequest, UpsertAccountCommandResponse> processor,
        ICommandValidator<UpsertAccountCommandRequest, UpsertAccountCommandResponse> validator
    ) : base(
        processor,
        validator)
    {
    }
}
