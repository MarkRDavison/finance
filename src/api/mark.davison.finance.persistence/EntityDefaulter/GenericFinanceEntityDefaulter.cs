namespace mark.davison.finance.persistence.EntityDefaulter;

public class GenericFinanceEntityDefaulter<TEntity> : IEntityDefaulter<TEntity>
    where TEntity : FinanceEntity
{
    private readonly IDateService _dateService;

    public GenericFinanceEntityDefaulter(IDateService dateService)
    {
        _dateService = dateService;
    }

    public Task DefaultAsync(TEntity entity, User user)
    {
        if (entity.UserId == default(Guid))
        {
            entity.UserId = user.Id;
        }

        if (entity.Created == default(DateTime))
        {
            entity.Created = _dateService.Now;
        }

        entity.LastModified = _dateService.Now;

        return Task.CompletedTask;
    }
}
