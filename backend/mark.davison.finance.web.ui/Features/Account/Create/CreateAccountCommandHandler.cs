using mark.davison.finance.bff.commands.Scenarios.CreateLocation;

namespace mark.davison.finance.web.ui.Features.Account.Create;

public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, CreateAccountCommandResult>
{
    private readonly IClientHttpRepository _repository;
    private readonly IStateStore _stateStore;

    public CreateAccountCommandHandler(
        IClientHttpRepository repository,
        IStateStore stateStore)
    {
        _repository = repository;
        _stateStore = stateStore;
    }

    public async Task<CreateAccountCommandResult> Handle(CreateAccountCommand command, CancellationToken cancellation)
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
        var response = await _repository.Post<CreateAccountResponse, CreateAccountRequest>(request);
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
