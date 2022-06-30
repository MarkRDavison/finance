using mark.davison.finance.bff.commands.Scenarios.CreateLocation;

namespace mark.davison.finance.web.ui.Features.Account;

public class CreateAccountAction : IRequest<CreateAccountActionResult>
{
    public string Name { get; set; } = string.Empty;
    public long? VirtualBalance { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public Guid BankId { get; set; }
    public Guid AccountTypeId { get; set; }
    public Guid CurrencyId { get; set; }
}
public class CreateAccountActionResult
{
    public bool Success { get; set; }

    public AccountListItemDto? Item { get; set; }
}

public class CreateAccountActionHandler : IRequestHandler<CreateAccountAction, CreateAccountActionResult>
{
    private readonly IStore _store;
    private readonly IClientHttpRepository _repository;

    public CreateAccountActionHandler(
        IStore store,
        IClientHttpRepository repository
    )
    {
        _store = store;
        _repository = repository;
    }

    public async Task<CreateAccountActionResult> Handle(CreateAccountAction request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var response = await _repository.Post<CreateAccountResponse, CreateAccountRequest>(new CreateAccountRequest
        {
        });

        return new CreateAccountActionResult
        {
        };
    }
}