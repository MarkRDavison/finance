using mark.davison.common.server.abstractions.Authentication;

namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankController : BaseController<Bank>
{
    public BankController(
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

    protected override void PatchUpdate(Bank persisted, Bank patched)
    {
        throw new NotImplementedException();
    }
}
