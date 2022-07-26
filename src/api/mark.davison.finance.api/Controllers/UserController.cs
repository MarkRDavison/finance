using mark.davison.common.server.abstractions.Authentication;
using mark.davison.common.server.abstractions.Identification;

namespace mark.davison.finance.api.Controllers;


[ApiController]
[Route("api/[controller]")]
public partial class UserController : BaseController<User>
{
    public UserController(
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

    protected override void PatchUpdate(User persisted, User patched)
    {
        throw new NotImplementedException();
    }
}
