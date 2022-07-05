namespace mark.davison.finance.bff.commands.Scenarios.CreateLocation;

public class CreateAccountCommandHandler : ICommandHandler<CreateAccountRequest, CreateAccountResponse>
{

    private readonly IHttpRepository _httpRepository;
    private readonly ICreateAccountCommandValidator _createAccountCommandValidator;

    public CreateAccountCommandHandler(
        IHttpRepository httpRepository,
        ICreateAccountCommandValidator createAccountCommandValidator
    )
    {
        _httpRepository = httpRepository;
        _createAccountCommandValidator = createAccountCommandValidator;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = await _createAccountCommandValidator.Validate(query, currentUserContext, cancellation);

        if (!response.Success)
        {
            return response;
        }

        var account = new Account
        {
            Id = query.Id,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            Name = query.Name,
            IsActive = true,
            VirtualBalance = query.VirtualBalance,
            AccountNumber = query.AccountNumber,
            AccountTypeId = query.AccountTypeId,
            BankId = query.BankId,
            CurrencyId = query.CurrencyId,
            Order = -1,
            UserId = currentUserContext.CurrentUser.Id
        };

        await _httpRepository.UpsertEntityAsync(
            account,
            HeaderParameters.Auth(
                currentUserContext.Token,
                currentUserContext.CurrentUser),
            cancellation);

        return response;
    }
}
