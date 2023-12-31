﻿namespace mark.davison.finance.bff.commands.Scenarios.CreateAccount.Validators;

public class UpsertAccountCommandValidator : IUpsertAccountCommandValidator
{

    private readonly IRepository _repository;

    public const string VALIDATION_ACCOUNT_TYPE_ID = "INVALID_ACCOUNT_TYPE_ID";
    public const string VALIDATION_CURRENCY_ID = "INVALID_CURRENCY_ID";

    public const string VALIDATION_MISSING_REQ_FIELD = "MISSING_REQ${0}";
    public const string VALIDATION_DUPLICATE_ACC_NUM = "DUPLICATE_ACC_NUM";
    public const string VALIDATION_MISSING_OPENING_BAL_DATE = "MISSING_OPENING_BAL_DATE";

    public UpsertAccountCommandValidator(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpsertAccountCommandResponse> Validate(UpsertAccountCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new UpsertAccountCommandResponse
        {
            Success = true
        };

        await using (_repository.BeginTransaction())
        {
            var accountTypeExists = await _repository.EntityExistsAsync<AccountType>(
                request.UpsertAccountDto.AccountTypeId,
                cancellationToken);

            if (!accountTypeExists)
            {
                response.Success = false;
                response.Error.Add(VALIDATION_ACCOUNT_TYPE_ID);
                return response;
            }

            var currencyExists = await _repository.EntityExistsAsync<Currency>(
                request.UpsertAccountDto.CurrencyId,
                cancellationToken);

            if (!currencyExists)
            {
                response.Success = false;
                response.Error.Add(VALIDATION_CURRENCY_ID);
                return response;
            }

            if (string.IsNullOrEmpty(request.UpsertAccountDto.Name))
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

            if (request.UpsertAccountDto.OpeningBalance != null && request.UpsertAccountDto.OpeningBalanceDate == null)
            {
                response.Success = false;
                response.Error.Add(VALIDATION_MISSING_OPENING_BAL_DATE);
                return response;
            }
        }

        return response;
    }

    internal async Task<bool> ValidateDuplicateAccount(UpsertAccountCommandRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UpsertAccountDto.AccountNumber))
        {
            return true;
        }

        Guid opposingGuid = Guid.Empty;
        if (request.UpsertAccountDto.AccountTypeId == AccountConstants.Expense)
        {
            opposingGuid = AccountConstants.Revenue;
        }
        else if (request.UpsertAccountDto.AccountTypeId == AccountConstants.Revenue)
        {
            opposingGuid = AccountConstants.Expense;
        }

        var duplicateAccounts = await _repository.GetEntitiesAsync<Account>(
            _ => _.UserId == currentUserContext.CurrentUser.Id,
            cancellationToken);

        foreach (var duplicateAccount in duplicateAccounts
            .Where(_ =>
                _.Id != request.UpsertAccountDto.Id &&
                _.AccountNumber == request.UpsertAccountDto.AccountNumber))
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

