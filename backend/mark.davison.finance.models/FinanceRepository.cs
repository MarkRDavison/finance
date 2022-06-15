namespace mark.davison.finance.models;

public class FinanceRepository : RepositoryBase<FinanceDbContext>
{
    public FinanceRepository(
        IDbContextFactory<FinanceDbContext> dbContextFactory, 
        ILogger<RepositoryBase<FinanceDbContext>> logger
    ) : base(
        dbContextFactory, 
        logger)
    {
    }
}
