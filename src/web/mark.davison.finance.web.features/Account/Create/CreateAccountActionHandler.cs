namespace mark.davison.finance.web.features.Account.Create;

public class CreateAccountActionHandler : ICommandHandler<CreateAccountCommandRequest, CreateAccountCommandResponse>
{
    private readonly IClientHttpRepository _repository;

    public CreateAccountActionHandler(
        IClientHttpRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommandRequest command, CancellationToken cancellationToken)
    {
        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = command.AccountNumber,
                AccountTypeId = command.AccountTypeId,
                CurrencyId = command.CurrencyId,
                Id = command.Id == default ? Guid.NewGuid() : command.Id,
                Name = command.Name,
                VirtualBalance = command.VirtualBalance,
                OpeningBalance = command.OpeningBalance,
                OpeningBalanceDate = command.OpeningBalanceDate
            }
        };
        var response = await _repository.Post<UpsertAccountCommandResponse, UpsertAccountCommandRequest>(request, cancellationToken);
        if (!response.Success)
        {
            return new CreateAccountCommandResponse { Success = false };
        }

        return new CreateAccountCommandResponse
        {
            Success = true,
            ItemId = request.UpsertAccountDto.Id
        };
    }
}
