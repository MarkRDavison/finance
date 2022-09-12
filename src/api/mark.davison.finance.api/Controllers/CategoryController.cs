namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : BaseFinanceController<Category>
{
    public CategoryController(
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

    protected override void PatchUpdate(Category persisted, Category patched)
    {
        throw new NotImplementedException();
    }
}
