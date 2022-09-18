namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount.Validators;

public class CreateAccountCommandValidator : ICreateAccountCommandValidator
{

    private readonly IHttpRepository _httpRepository;

    public const string VALIDATION_ACCOUNT_TYPE_ID = "INVALID_ACCOUNT_TYPE_ID";
    public const string VALIDATION_CURRENCY_ID = "INVALID_CURRENCY_ID";

    public const string VALIDATION_MISSING_REQ_FIELD = "MISSING_REQ${0}";
    public const string VALIDATION_DUPLICATE_ACC_NUM = "DUPLICATE_ACC_NUM";
    public const string VALIDATION_MISSING_OPENING_BAL_DATE = "MISSING_OPENING_BAL_DATE";

    public CreateAccountCommandValidator(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<CreateAccountResponse> Validate(CreateAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateAccountResponse
        {
            Success = true
        };

        var authHeaders = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);

        var accountType = await _httpRepository.GetEntityAsync<AccountType>(
            request.CreateAccountDto.AccountTypeId,
            authHeaders,
            cancellationToken);

        if (accountType == null)
        {
            response.Success = false;
            response.Error.Add(VALIDATION_ACCOUNT_TYPE_ID);
            return response;
        }

        var currency = await _httpRepository.GetEntityAsync<Currency>(
            request.CreateAccountDto.CurrencyId,
            authHeaders,
            cancellationToken);

        if (currency == null)
        {
            response.Success = false;
            response.Error.Add(VALIDATION_CURRENCY_ID);
            return response;
        }

        if (string.IsNullOrEmpty(request.CreateAccountDto.Name))
        {
            response.Success = false;
            response.Error.Add(string.Format(VALIDATION_MISSING_REQ_FIELD, nameof(Account.Name)));
            return response;
        }

        if (!await ValidateDuplicateAccount(request, currentUserContext, cancellationToken))
        {
            response.Success = false;
            response.Error.Add(VALIDATION_DUPLICATE_ACC_NUM);
            return response;
        }

        if (request.CreateAccountDto.OpeningBalance != null && request.CreateAccountDto.OpeningBalanceDate == null)
        {
            response.Success = false;
            response.Error.Add(VALIDATION_MISSING_OPENING_BAL_DATE);
            return response;
        }

        return response;
    }

    internal async Task<bool> ValidateDuplicateAccount(CreateAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.CreateAccountDto.AccountNumber))
        {
            return true;
        }

        Guid opposingGuid = Guid.Empty;
        if (request.CreateAccountDto.AccountTypeId == AccountConstants.Expense)
        {
            opposingGuid = AccountConstants.Revenue;
        }
        else if (request.CreateAccountDto.AccountTypeId == AccountConstants.Revenue)
        {
            opposingGuid = AccountConstants.Expense;
        }

        var query = new QueryParameters
        {
            { nameof(Account.UserId), currentUserContext.CurrentUser.Id.ToString() },
            { nameof(Account.AccountNumber), request.CreateAccountDto.AccountNumber }
        };

        var duplicateAccounts = await _httpRepository.GetEntitiesAsync<Account>(
            query,
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellationToken);

        foreach (var duplicateAccount in duplicateAccounts)
        {
            if (opposingGuid == Guid.Empty)
            {
                return false;
            }

            if (duplicateAccount.AccountTypeId != opposingGuid)
            {
                return false;
            }
        }

        return true;
    }
}

