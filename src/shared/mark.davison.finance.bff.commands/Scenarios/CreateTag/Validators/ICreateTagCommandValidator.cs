namespace mark.davison.finance.bff.commands.Scenarios.CreateTag.Validators;

public interface ICreateTagCommandValidator
{
    Task<CreateTagCommandResponse> Validate(CreateTagCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken);
}
