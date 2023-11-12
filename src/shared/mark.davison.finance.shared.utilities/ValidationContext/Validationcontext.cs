using mark.davison.common.server.abstractions.Repository;

namespace mark.davison.finance.shared.utilities.ValidationContext;

public class Validationcontext : IValidationContext
{
    private readonly IRepository _repository;

    private readonly IDictionary<Type, IDictionary<Guid, BaseEntity?>> _cache;

    public Validationcontext(
        IRepository repository
    )
    {
        _repository = repository;

        _cache = new Dictionary<Type, IDictionary<Guid, BaseEntity?>>();
    }

    public async Task<T?> GetById<T>(Guid id, CancellationToken cancellationToken)
        where T : BaseEntity, new()
    {
        IDictionary<Guid, BaseEntity?>? entityCache;

        if (!_cache.TryGetValue(typeof(T), out entityCache))
        {
            entityCache = new Dictionary<Guid, BaseEntity?>();
        }

        T? entity;

        if (entityCache.TryGetValue(id, out var baseEntity))
        {
            entity = baseEntity as T;
        }
        else
        {
            entity = await _repository.GetEntityAsync<T>(id, cancellationToken);
            entityCache.Add(id, entity);
        }

        return entity;
    }
}
