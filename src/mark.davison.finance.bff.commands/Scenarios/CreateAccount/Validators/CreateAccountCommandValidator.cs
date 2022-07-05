using mark.davison.finance.common.server.abstractions.Authentication;

namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount.Validators;

public class CreateAccountCommandValidator : ICreateAccountCommandValidator
{

    private readonly IHttpRepository _httpRepository;

    public const string VALIDATION_BANK_ID = "INVALID_BANK_ID";
    public const string VALIDATION_ACCOUNT_TYPE_ID = "INVALID_ACCOUNT_TYPE_ID";
    public const string VALIDATION_CURRENCY_ID = "INVALID_CURRENCY_ID";

    public const string VALIDATION_MISSING_REQ_FIELD = "MISSING_REQ${0}";
    public const string VALIDATION_DUPLICATE_ACC_NUM = "DUPLICATE_ACC_NUM";

    public CreateAccountCommandValidator(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<CreateAccountResponse> Validate(CreateAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = new CreateAccountResponse
        {
            Success = true
        };

        var authHeaders = HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser);

        var bank = await _httpRepository.GetEntityAsync<Bank>(
            request.BankId,
            authHeaders,
            cancellation);

        if (bank == null)
        {
            response.Success = false;
            response.Error.Add(VALIDATION_BANK_ID);
            return response;
        }

        var accountType = await _httpRepository.GetEntityAsync<AccountType>(
            request.AccountTypeId,
            authHeaders,
            cancellation);

        if (accountType == null)
        {
            response.Success = false;
            response.Error.Add(VALIDATION_ACCOUNT_TYPE_ID);
            return response;
        }

        var currency = await _httpRepository.GetEntityAsync<Currency>(
            request.CurrencyId,
            authHeaders,
            cancellation);

        if (currency == null)
        {
            response.Success = false;
            response.Error.Add(VALIDATION_CURRENCY_ID);
            return response;
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            response.Success = false;
            response.Error.Add(string.Format(VALIDATION_MISSING_REQ_FIELD, nameof(Account.Name)));
            return response;
        }

        if (!await ValidateDuplicateAccount(request, currentUserContext, cancellation))
        {
            response.Success = false;
            response.Error.Add(VALIDATION_DUPLICATE_ACC_NUM);
        }

        return response;
    }

    internal async Task<bool> ValidateDuplicateAccount(CreateAccountRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        if (string.IsNullOrEmpty(request.AccountNumber))
        {
            return true;
        }

        Guid opposingGuid = Guid.Empty;
        if (request.AccountTypeId == AccountType.Expense)
        {
            opposingGuid = AccountType.Revenue;
        }
        else if (request.AccountTypeId == AccountType.Revenue)
        {
            opposingGuid = AccountType.Expense;
        }

        var query = new QueryParameters
        {
            { nameof(Account.UserId), currentUserContext.CurrentUser.Id.ToString() },
            { nameof(Account.AccountNumber), request.AccountNumber }
        };

        var duplicateAccounts = await _httpRepository.GetEntitiesAsync<Account>(
            query,
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

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

