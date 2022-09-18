namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Validators;

public class CreateTransactionCommandValidator : ICreateTransactionCommandValidator
{
    public const string VALIDATION_TRANSACTION_TYPE = "INVALID_TRANSACTION_TYPE";
    public const string VALIDATION_CURRENCY_ID = "INVALID_CURRENCY_ID${0}";
    public const string VALIDATION_FOREIGN_CURRENCY_ID = "INVALID_FOREIGN_CURRENCY_ID${0}";
    public const string VALIDATION_DUPLICATE_CURRENCY = "INVALID_DUPLICATE_CURRENCY${0}";
    public const string VALIDATION_GROUP_DESCRIPTION = "INVALID_GROUP_DESCR";
    public const string VALIDATION_CATEGORY_ID = "INVALID_CATEGORYID${0}";
    public const string VALIDATION_DATE = "INVALID_DATE${0}";
    public const string VALIDATION_DUPLICATE_SRC_DEST_ACCOUNT = "DUP_ACT${0}";

    private readonly IHttpRepository _httpRepository;
    private readonly ICreateTransctionValidationContext _createTransctionValidationContext;
    private readonly ICreateTransactionValidatorStrategyFactory _createTransactionValidatorStrategyFactory;

    public CreateTransactionCommandValidator(
        IHttpRepository httpRepository,
        ICreateTransctionValidationContext createTransctionValidationContext,
        ICreateTransactionValidatorStrategyFactory createTransactionValidatorStrategyFactory)
    {
        _httpRepository = httpRepository;
        _createTransctionValidationContext = createTransctionValidationContext;
        _createTransactionValidatorStrategyFactory = createTransactionValidatorStrategyFactory;
    }

    public async Task<CreateTransactionResponse> Validate(CreateTransactionRequest request, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new CreateTransactionResponse();

        var transctionTypeValidator = _createTransactionValidatorStrategyFactory.CreateStrategy(request.TransactionTypeId);

        if (!TransactionConstants.Ids.Contains(request.TransactionTypeId))
        {
            response.Error.Add(VALIDATION_TRANSACTION_TYPE);
        }

        if (request.Transactions.Count > 1 && string.IsNullOrEmpty(request.Description))
        {
            response.Error.Add(VALIDATION_GROUP_DESCRIPTION);
        }

        await transctionTypeValidator.ValidateTransactionGroup(request, response, _createTransctionValidationContext);

        foreach (var transaction in request.Transactions)
        {
            await ValidateTransaction(response, transaction, currentUserContext, cancellationToken);
            await transctionTypeValidator.ValidateTranasction(transaction, response, _createTransctionValidationContext);
        }

        response.Success = !response.Error.Any();

        return response;
    }

    private async Task ValidateTransaction(CreateTransactionResponse response, CreateTransactionDto transaction, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        if (transaction.Date == default)
        {
            response.Error.Add(string.Format(VALIDATION_DATE, transaction.Id));
        }

        if (transaction.SourceAccountId == transaction.DestinationAccountId)
        {
            response.Error.Add(string.Format(VALIDATION_DUPLICATE_SRC_DEST_ACCOUNT, transaction.Id));
        }

        if (transaction.CategoryId != null && await _createTransctionValidationContext.GetCategoryById(transaction.CategoryId.Value, cancellationToken) == null)
        {
            response.Error.Add(string.Format(VALIDATION_CATEGORY_ID, transaction.Id));
        }

        if (!Currency.Ids.Contains(transaction.CurrencyId))
        {
            response.Error.Add(string.Format(VALIDATION_CURRENCY_ID, transaction.Id));
        }

        if (transaction.ForeignCurrencyId.HasValue && !Currency.Ids.Contains(transaction.ForeignCurrencyId.Value))
        {
            response.Error.Add(string.Format(VALIDATION_FOREIGN_CURRENCY_ID, transaction.Id));
        }

        if (transaction.ForeignCurrencyId.HasValue &&
            transaction.ForeignCurrencyId.Value == transaction.CurrencyId)
        {
            response.Error.Add(string.Format(VALIDATION_DUPLICATE_CURRENCY, transaction.Id));
        }
    }
}
