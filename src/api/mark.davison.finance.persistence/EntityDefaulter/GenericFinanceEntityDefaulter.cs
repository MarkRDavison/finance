namespace mark.davison.finance.persistence.EntityDefaulter;
public class GenericFinanceEntityDefaulter<TEntity> : IEntityDefaulter<TEntity>
    where TEntity : FinanceEntity
{
    public Task DefaultAsync(TEntity entity, User user)
    {
        if (entity.UserId == default(Guid))
        {
            entity.UserId = user.Id;
        }

        if (entity.Created == default(DateTime))
        {
            entity.Created = DateTime.UtcNow;
        }

        entity.LastModified = DateTime.UtcNow;

        return Task.CompletedTask;
    }
}
