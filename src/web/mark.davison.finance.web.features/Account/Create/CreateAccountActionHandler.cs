using mark.davison.finance.bff.commands.Scenarios.CreateLocation;

namespace mark.davison.finance.web.features.Account.Create;

public class CreateAccountActionHandler : ICommandHandler<CreateAccountAction, CreateAccountCommandResult>
{
    private readonly IClientHttpRepository _repository;

    public CreateAccountActionHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateAccountCommandResult> Handle(CreateAccountAction command, CancellationToken cancellation)
    {
        var request = new CreateAccountRequest
        {
            AccountNumber = command.AccountNumber,
            AccountTypeId = command.AccountTypeId,
            BankId = command.BankId,
            CurrencyId = command.CurrencyId,
            Id = Guid.NewGuid(),
            Name = command.Name,
            VirtualBalance = command.VirtualBalance
        };
        var response = await _repository.Post<CreateAccountResponse, CreateAccountRequest>(request, cancellation);
        if (!response.Success)
        {
            return new CreateAccountCommandResult { Success = false };
        }

        return new CreateAccountCommandResult
        {
            Success = true,
            ItemId = request.Id
        };
    }
}
