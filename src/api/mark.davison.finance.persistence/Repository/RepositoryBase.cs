﻿namespace mark.davison.finance.persistence.Repository;

public abstract class RepositoryBase<TContext> : IRepository
    where TContext : DbContext
{
    private readonly ILogger<RepositoryBase<TContext>> _logger;
    private readonly IDbContextFactory<TContext> _dbContextFactory;

    protected RepositoryBase(
        IDbContextFactory<TContext> dbContextFactory,
        ILogger<RepositoryBase<TContext>> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }


    public Task<List<T>> GetEntitiesAsync<T>(
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return GetEntitiesAsync<T>(null, null, cancellationToken);
    }

    public Task<List<T>> GetEntitiesAsync<T>(
        Expression<Func<T, bool>>? predicate,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return GetEntitiesAsync<T>(predicate, null, cancellationToken);
    }

    public Task<List<T>> GetEntitiesAsync<T>(
        Expression<Func<T, object>>[]? includes,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return GetEntitiesAsync<T>(null, includes, cancellationToken);
    }

    public Task<List<T>> GetEntitiesAsync<T>(
        Expression<Func<T, bool>>? predicate,
        Expression<Func<T, object>>[]? includes,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return GetEntitiesAsync<T, T>(predicate, includes, null, cancellationToken);
    }

    public async Task<List<TProjection>> GetEntitiesAsync<TEntity, TProjection>(
        Expression<Func<TEntity, bool>>? predicate,
        Expression<Func<TEntity, object>>[]? includes,
        Expression<Func<TEntity, TProjection>>? projection,
        CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        return await (await QueryUnitOfWorkAsync(predicate, includes, projection, cancellationToken)).ToListAsync(cancellationToken);
    }

    public Task<T?> GetEntityAsync<T>(
        Guid id,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return GetEntityAsync<T>(id, null, cancellationToken);
    }

    public Task<T?> GetEntityAsync<T>(
        Guid id,
        Expression<Func<T, object>>[]? include,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return GetEntityAsync<T>(_ => _.Id == id, include, cancellationToken);
    }

    public Task<T?> GetEntityAsync<T>(
        Expression<Func<T, bool>>? predicate,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return GetEntityAsync<T>(predicate, null, cancellationToken);
    }

    public async Task<T?> GetEntityAsync<T>(
        Expression<Func<T, bool>>? predicate,
        Expression<Func<T, object>>[]? include,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return await (await QueryUnitOfWorkAsync(predicate, include, _ => _, cancellationToken)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<T>> UpsertEntitiesAsync<T>(
        List<T> entities,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        //Check that there are no entities in the list with duplicate Id's that are not Guid.Empty
        var containsInvalidDuplicates = entities
            .Where(_ => !_.Id.Equals(Guid.Empty))
            .GroupBy(_ => _.Id)
            .Any(_ => _.Count() > 1);
        if (containsInvalidDuplicates)
        {
            throw new InvalidOperationException("Duplicate entities detected.");
        }

        var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var set = context.Set<T>();

        var addBatchItems = entities
            .Where(_ => _.Id.Equals(Guid.Empty))
            .ToList();

        set.AddRange(addBatchItems);

        var upsertBatchItems = entities
            .Where(_ => !_.Id.Equals(Guid.Empty))
            .ToList();
        var upsertBatchItemsIds = upsertBatchItems
            .Select(_ => _.Id)
            .ToArray();

        var existingBatchItems = await set
            .Where(_ => upsertBatchItemsIds.Contains(_.Id))
            .ToListAsync(cancellationToken);

        foreach (var upsertBatchItem in upsertBatchItems)
        {
            upsertBatchItem.LastModified = DateTime.UtcNow;
            var existingBatchItem = existingBatchItems.Find(_ => _.Id.Equals(upsertBatchItem.Id));
            if (existingBatchItem == null)
            {
                upsertBatchItem.Created = DateTime.UtcNow;
                set.Add(upsertBatchItem);
            }
            else
            {
                context.Entry(existingBatchItem).CurrentValues.SetValues(upsertBatchItem);
            }
        }

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("UpsertEntitiesAsync<{0}> - {1}", nameof(T), e);
            throw;
        }


        return entities;
    }

    public async Task<T?> UpsertEntityAsync<T>(
        T entity,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var set = context.Set<T>();
        var existingEntity = await set.FindAsync(new object[] { entity.Id }, cancellationToken: cancellationToken);
        if (existingEntity == null)
        {
            entity.Created = DateTime.UtcNow;
            entity.LastModified = DateTime.UtcNow;
            set.Add(entity);
        }
        else
        {
            //SetValues will only mark as modified the properties that have different
            //values to those in the tracked entity. This means that when the update
            //is sent, only those columns that have actually changed will be updated.
            //(And if nothing has changed, then no update will be sent at all.)
            entity.LastModified = DateTime.UtcNow;
            context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        var saveChangesResult = await context.SaveChangesAsync();
        if (saveChangesResult != 1)
        {
            return null;
        }

        await context.DisposeAsync();
        return entity;
    }

    public Task<T?> DeleteEntityAsync<T>(
        T entity,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return DeleteUnitOfWorkAsync<T>(entity, cancellationToken);
    }

    public async Task<List<T>> DeleteEntitiesAsync<T>(
        List<T> entities,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Set<T>();

        var itemsToDelete = entities
            .Where(_ => !_.Id.Equals(Guid.Empty))
            .ToList();
        var itemsToDeleteIds = itemsToDelete
            .Select(_ => _.Id)
            .ToArray();

        var entitiesToDelete = await GetEntitiesAsync<T>(_ => itemsToDeleteIds.Contains(_.Id), cancellationToken)
            .ConfigureAwait(false);

        try
        {
            context.RemoveRange(entitiesToDelete);
            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return itemsToDelete;
        }
        catch (Exception e)
        {
            _logger.LogError("DeleteEntitiesAsync<{0}> - {1}", nameof(T), e);
            throw;
        }
    }

    private async Task<IQueryable<TProjection>> QueryUnitOfWorkAsync<TEntity, TProjection>(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>[]? includes = null,
        Expression<Func<TEntity, TProjection>>? projection = null,
        CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var set = context.Set<TEntity>();

        var query = set.Where(_ => true).AsNoTracking();

        query = AttachIncludes<TEntity>(query, includes);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        try
        {
            IQueryable<TProjection> result;

            if (projection != null)
            {
                result = query
                    .Select(projection);
            }
            else
            {
                result = query
                    .OfType<TProjection>();
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("QueryUnitOfWorkAsync<{0}> - {1}", nameof(TEntity), e);
            throw;
        }
    }

    private async Task<TEntity?> DeleteUnitOfWorkAsync<TEntity>(
        TEntity item,
        CancellationToken cancellationToken = default)
        where TEntity : BaseEntity
    {
        var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Set<TEntity>();

        var existingItem = await GetEntityAsync<TEntity>(item.Id, null, cancellationToken)
            .ConfigureAwait(false);

        if (existingItem != null)
        {
            context.Entry(item).State = EntityState.Deleted;
        }
        else
        {
            return null;
        }

        try
        {
            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return item;
        }
        catch (Exception e)
        {
            _logger.LogError("DeleteUnitOfWorkAsync<{0}> - {1}", nameof(TEntity), e);
            throw;
        }
    }

    private static IQueryable<TEntity> AttachIncludes<TEntity>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[]? includes)
        where TEntity : BaseEntity
    {
        if (includes == null)
        {
            return query;
        }

        foreach (var include in includes)
        {
            if (include.Body is not MethodCallExpression expression)
            {
                query = query.Include(include);
                continue;
            }

            var arguments = expression.Arguments;

            if (arguments.Count <= 1)
            {
                query = query.Include(include);
                continue;
            }

            var parts = arguments.Select(_ =>
            {
                var startIndex = _.ToString().IndexOf('.') + 1;
                return _.ToString()[startIndex..];
            });

            var navigationPath = string.Join(".", parts);
            query = query.Include(navigationPath);
        }

        return query;
    }
}