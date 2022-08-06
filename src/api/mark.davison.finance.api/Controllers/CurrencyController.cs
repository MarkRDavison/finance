namespace mark.davison.finance.api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CurrencyController : BaseFinanceController<Currency>
{
    public CurrencyController(
        ILogger<UserController> logger,
        IRepository repository,
        IServiceScopeFactory serviceScopeFactory,
        ICurrentUserContext currentUserContext
    ) : base(
        logger,
        repository,
        serviceScopeFactory,
        currentUserContext
    )
    {
    }

    protected override void PatchUpdate(Currency persisted, Currency patched)
    {
        throw new NotImplementedException();
    }
}
