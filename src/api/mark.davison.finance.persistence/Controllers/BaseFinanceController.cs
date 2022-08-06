namespace mark.davison.finance.persistence.Controllers;

public abstract class BaseFinanceController<TEntity> : BaseController<TEntity> where TEntity : FinanceEntity, new()
{
    protected BaseFinanceController(ILogger logger, IRepository repository, IServiceScopeFactory serviceScopeFactory, ICurrentUserContext currentUserContext) : base(logger, repository, serviceScopeFactory, currentUserContext)
    {
    }

}
