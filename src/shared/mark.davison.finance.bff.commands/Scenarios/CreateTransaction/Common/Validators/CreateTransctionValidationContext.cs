namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common.Validators;

public class CreateTransctionValidationContext : ICreateTransctionValidationContext
{
    private readonly IHttpRepository _httpRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly Dictionary<Guid, Account?> _accountCache;
    private readonly Dictionary<Guid, Category?> _categoryCache;

    public CreateTransctionValidationContext(
        IHttpRepository httpRepository,
        ICurrentUserContext currentUserContext)
    {
        _httpRepository = httpRepository;
        _currentUserContext = currentUserContext;
        _accountCache = new();
        _categoryCache = new();
    }

    public async Task<Account?> GetAccountById(Guid accountId, CancellationToken cancellationToken)
    {
        if (!_accountCache.ContainsKey(accountId))
        {
            var account = await _httpRepository.GetEntityAsync<Account>(
                accountId,
                HeaderParameters.Auth(_currentUserContext.Token, _currentUserContext.CurrentUser),
                cancellationToken);
            _accountCache[accountId] = account;
        }

        return _accountCache[accountId];
    }

    public async Task<Category?> GetCategoryById(Guid categoryId, CancellationToken cancellationToken)
    {
        if (!_categoryCache.ContainsKey(categoryId))
        {
            var category = await _httpRepository.GetEntityAsync<Category>(
                categoryId,
                HeaderParameters.Auth(_currentUserContext.Token, _currentUserContext.CurrentUser),
                cancellationToken);
            _categoryCache[categoryId] = category;
        }

        return _categoryCache[categoryId];
    }
}
