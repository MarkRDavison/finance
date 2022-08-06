namespace mark.davison.finance.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionJournalController : BaseFinanceController<TransactionJournal>
{
    public TransactionJournalController(
        ILogger<TransactionJournalController> logger,
        IRepository repository,
        IServiceScopeFactory serviceScopeFactory,
        ICurrentUserContext currentUserContext
    ) : base(
        logger,
        repository,
        serviceScopeFactory,
        currentUserContext)
    {
    }

    protected override void PatchUpdate(TransactionJournal persisted, TransactionJournal patched)
    {
        throw new NotImplementedException();
    }
}
