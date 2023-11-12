namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

public class CreateTransactionCommandValidator : ICommandValidator<CreateTransactionRequest, CreateTransactionResponse>
{
    // TODO: Move to validation messages class
    public const string VALIDATION_TRANSACTION_TYPE = "INVALID_TRANSACTION_TYPE";
    public const string VALIDATION_CURRENCY_ID = "INVALID_CURRENCY_ID${0}";
    public const string VALIDATION_FOREIGN_CURRENCY_ID = "INVALID_FOREIGN_CURRENCY_ID${0}";
    public const string VALIDATION_DUPLICATE_CURRENCY = "INVALID_DUPLICATE_CURRENCY${0}";
    public const string VALIDATION_GROUP_DESCRIPTION = "INVALID_GROUP_DESCR";
    public const string VALIDATION_CATEGORY_ID = "INVALID_CATEGORYID${0}";
    public const string VALIDATION_DATE = "INVALID_DATE${0}";
    public const string VALIDATION_DUPLICATE_SRC_DEST_ACCOUNT = "DUP_ACT${0}";
    public const string VALIDATION_DUPLICATE_TAGS = "DUP_ACT${0}";
    public const string VALIDATION_INVALID_DESTINATION_ACCOUNT_TYPE = "INVALID_DEST_ACCT_TYPE";
    public const string VALIDATION_INVALID_ACCOUNT_PAIR = "INVALID_ACCT_PAIR";
    public const string VALIDATION_INVALID_SOURCE_ACCOUNT_TYPE = "INVALID_SRC_ACCT_TYPE";

    private readonly ICreateTransactionValidatorStrategyFactory _createTransactionValidatorStrategyFactory;
    private readonly ICreateTransctionValidationContext _createTransctionValidationContext;

    public CreateTransactionCommandValidator(
        ICreateTransactionValidatorStrategyFactory createTransactionValidatorStrategyFactory,
        ICreateTransctionValidationContext createTransctionValidationContext
    )
    {
        _createTransactionValidatorStrategyFactory = createTransactionValidatorStrategyFactory;
        _createTransctionValidationContext = createTransctionValidationContext;
    }

    public async Task<CreateTransactionResponse> ValidateAsync(CreateTransactionRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTransactionResponse();

        var transctionTypeValidator = _createTransactionValidatorStrategyFactory.CreateStrategy(request.TransactionTypeId);

        if (!TransactionConstants.Ids.Contains(request.TransactionTypeId))
        {
            response.Errors.Add(VALIDATION_TRANSACTION_TYPE);
        }

        if (request.Transactions.Count > 1 && string.IsNullOrEmpty(request.Description))
        {
            response.Errors.Add(VALIDATION_GROUP_DESCRIPTION);
        }

        await transctionTypeValidator.ValidateTransactionGroup(request, response, _createTransctionValidationContext);

        foreach (var transaction in request.Transactions)
        {
            await ValidateTransaction(response, transaction, currentUserContext, cancellationToken);
            await transctionTypeValidator.ValidateTranasction(transaction, response, _createTransctionValidationContext);
        }

        return response;
    }

    private async Task ValidateTransaction(CreateTransactionResponse response, CreateTransactionDto transaction, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        if (transaction.Date == default)
        {
            response.Errors.Add(string.Format(VALIDATION_DATE, transaction.Id));
        }

        if (transaction.SourceAccountId == transaction.DestinationAccountId)
        {
            response.Errors.Add(string.Format(VALIDATION_DUPLICATE_SRC_DEST_ACCOUNT, transaction.Id));
        }

        if (transaction.CategoryId != null)
        {

            if (null == await _createTransctionValidationContext.GetCategoryById(transaction.CategoryId.Value, cancellationToken))
            {
                response.Errors.Add(string.Format(VALIDATION_CATEGORY_ID, transaction.Id));
            }
        }

        if (!Currency.Ids.Contains(transaction.CurrencyId))
        {
            response.Errors.Add(string.Format(VALIDATION_CURRENCY_ID, transaction.Id));
        }

        if (transaction.ForeignCurrencyId.HasValue && !Currency.Ids.Contains(transaction.ForeignCurrencyId.Value))
        {
            response.Errors.Add(string.Format(VALIDATION_FOREIGN_CURRENCY_ID, transaction.Id));
        }

        if (transaction.ForeignCurrencyId.HasValue &&
            transaction.ForeignCurrencyId.Value == transaction.CurrencyId)
        {
            response.Errors.Add(string.Format(VALIDATION_DUPLICATE_CURRENCY, transaction.Id));
        }

        var duplicates = transaction.Tags.GroupBy(_ => _)
              .Where(_ => _.Count() > 1)
              .Select(_ => _.Key)
              .ToList();
        if (duplicates.Any())
        {
            response.Warnings.Add(string.Format(VALIDATION_DUPLICATE_TAGS, string.Join(",", duplicates)));
        }
    }
}
