using mark.davison.finance.common.server.abstractions.Authentication;

namespace mark.davison.finance.api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CurrencyController : BaseController<Currency>
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
