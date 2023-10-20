namespace mark.davison.finance.bff.commands.Scenarios.CreateTransaction.Common;

// TODO: Replace with ValidationContext
public class CreateTransctionValidationContext : ICreateTransctionValidationContext
{
    private readonly IRepository _repository;
    private readonly ICurrentUserContext _currentUserContext;

    private readonly IDictionary<Guid, Account?> _accounts;
    private readonly IDictionary<Guid, Category?> _categories;

    public CreateTransctionValidationContext(
        IRepository repository,
        ICurrentUserContext currentUserContext
    )
    {
        _repository = repository;
        _currentUserContext = currentUserContext;

        _accounts = new Dictionary<Guid, Account?>();
        _categories = new Dictionary<Guid, Category?>();
    }

    public Task<Account?> GetAccountById(Guid accountId, CancellationToken cancellationToken) => GetEntityById(accountId, _accounts, cancellationToken);

    public Task<Category?> GetCategoryById(Guid categoryId, CancellationToken cancellationToken) => GetEntityById(categoryId, _categories, cancellationToken);

    private async Task<T?> GetEntityById<T>(Guid id, IDictionary<Guid, T?> cache, CancellationToken cancellationToken)
        where T : BaseEntity, new()
    {
        if (cache.TryGetValue(id, out var entity))
        {
            return entity;
        }

        await using (_repository.BeginTransaction())
        {
            var newEntity = await _repository.GetEntityAsync<T>(id, cancellationToken);

            cache.Add(id, newEntity);

            return newEntity;
        }
    }
}
