using mark.davison.finance.models.dtos.Commands.CreateAccount;

namespace mark.davison.finance.web.features.Account.Create;

public class CreateAccountActionHandler : ICommandHandler<CreateAccountAction, CreateAccountCommandResponse>
{
    private readonly IClientHttpRepository _repository;

    public CreateAccountActionHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateAccountCommandResponse> Handle(CreateAccountAction command, CancellationToken cancellation)
    {
        var request = new CreateAccountRequest
        {
            CreateAccountDto = new CreateAccountDto
            {
                AccountNumber = command.AccountNumber,
                AccountTypeId = command.AccountTypeId,
                CurrencyId = command.CurrencyId,
                Id = Guid.NewGuid(),
                Name = command.Name,
                VirtualBalance = command.VirtualBalance
            }
        };
        var response = await _repository.Post<CreateAccountResponse, CreateAccountRequest>(request, cancellation);
        if (!response.Success)
        {
            return new CreateAccountCommandResponse { Success = false };
        }

        return new CreateAccountCommandResponse
        {
            Success = true,
            ItemId = request.CreateAccountDto.Id
        };
    }
}
