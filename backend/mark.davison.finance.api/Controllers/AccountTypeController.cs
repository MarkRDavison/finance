namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountTypeController : BaseController<AccountType>
{
    public AccountTypeController(
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

    protected override void PatchUpdate(AccountType persisted, AccountType patched)
    {
        throw new NotImplementedException();
    }
}
