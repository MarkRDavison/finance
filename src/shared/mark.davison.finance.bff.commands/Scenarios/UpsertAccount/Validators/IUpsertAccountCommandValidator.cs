using mark.davison.finance.models.dtos.Commands.UpsertAccount;

namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount.Validators;

public interface IUpsertAccountCommandValidator
{
    Task<UpsertAccountResponse> Validate(UpsertAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken);
}

