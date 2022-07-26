using mark.davison.common.server.abstractions.Authentication;

namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount.Validators;

public interface ICreateAccountCommandValidator
{
    Task<CreateAccountResponse> Validate(CreateAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellation);
}

