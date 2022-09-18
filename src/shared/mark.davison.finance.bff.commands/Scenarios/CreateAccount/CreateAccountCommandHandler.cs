namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount;

public class CreateAccountCommandHandler : ICommandHandler<CreateAccountRequest, CreateAccountResponse>
{

    private readonly IHttpRepository _httpRepository;
    private readonly ICreateAccountCommandValidator _createAccountCommandValidator;
    private readonly ICommandHandler<CreateTransactionRequest, CreateTransactionResponse> _createTransactionCommandHandler;

    public CreateAccountCommandHandler(
        IHttpRepository httpRepository,
        ICreateAccountCommandValidator createAccountCommandValidator,
        ICommandHandler<CreateTransactionRequest, CreateTransactionResponse> createTransactionCommandHandler
    )
    {
        _httpRepository = httpRepository;
        _createAccountCommandValidator = createAccountCommandValidator;
        _createTransactionCommandHandler = createTransactionCommandHandler;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = await _createAccountCommandValidator.Validate(request, currentUserContext, cancellationToken);

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
            cancellationToken);

        if (request.CreateAccountDto.OpeningBalance != null &&
            request.CreateAccountDto.OpeningBalanceDate != null)
        {
            await _createTransactionCommandHandler.Handle(new()
            {
                TransactionTypeId = TransactionConstants.OpeningBalance,
                Transactions =
                {
                    new CreateTransactionDto
                    {
                        Id = Guid.NewGuid(),
                        Amount = request.CreateAccountDto.OpeningBalance.Value,
                        Description = "Opening balance",
                        CurrencyId = account.CurrencyId,
                        SourceAccountId = Account.OpeningBalance,
                        DestinationAccountId = account.Id,
                        Date = request.CreateAccountDto.OpeningBalanceDate.Value
                    }
                }
            }, currentUserContext, cancellationToken);
        }

        return response;
    }
}
