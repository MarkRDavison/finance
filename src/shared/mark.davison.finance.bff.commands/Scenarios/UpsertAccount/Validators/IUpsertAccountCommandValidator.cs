using mark.davison.finance.models.dtos.Commands.UpsertAccount;

namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount.Validators;

public interface IUpsertAccountCommandValidator
{
    Task<UpsertAccountCommandResponse> Validate(UpsertAccountCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken);
}

