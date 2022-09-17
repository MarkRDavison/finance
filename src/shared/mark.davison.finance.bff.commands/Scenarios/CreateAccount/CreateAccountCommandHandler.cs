namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount;

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

    public async Task<CreateAccountResponse> Handle(CreateAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = await _createAccountCommandValidator.Validate(request, currentUserContext, cancellation);

        if (!response.Success)
        {
            return response;
        }

        var account = new Account
        {
            Id = request.CreateAccountDto.Id,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            Name = request.CreateAccountDto.Name,
            IsActive = true,
            VirtualBalance = request.CreateAccountDto.VirtualBalance,
            AccountNumber = request.CreateAccountDto.AccountNumber,
            AccountTypeId = request.CreateAccountDto.AccountTypeId,
            CurrencyId = request.CreateAccountDto.CurrencyId,
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
