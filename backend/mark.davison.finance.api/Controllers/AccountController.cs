namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : BaseController<Account>
{
    public AccountController(
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

    protected override void PatchUpdate(Account persisted, Account patched)
    {
        throw new NotImplementedException();
    }
}
