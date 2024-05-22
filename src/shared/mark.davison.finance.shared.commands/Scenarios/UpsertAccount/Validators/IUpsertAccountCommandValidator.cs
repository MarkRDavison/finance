namespace mark.davison.finance.shared.commands.Scenarios.CreateAccount.Validators;

public interface IUpsertAccountCommandValidator
{
    Task<UpsertAccountCommandResponse> Validate(UpsertAccountCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken);
}

