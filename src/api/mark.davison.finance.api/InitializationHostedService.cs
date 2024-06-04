namespace mark.davison.finance.api;

public class InitializationHostedService : GenericApplicationHealthStateHostedService
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;
    private readonly IOptions<AppSettings> _appSettings;

    public InitializationHostedService(
        IHostApplicationLifetime hostApplicationLifetime,
        IApplicationHealthState applicationHealthState,
        IDbContextFactory<FinanceDbContext> dbContextFactory,
        IOptions<AppSettings> appSettings
    ) : base(
        hostApplicationLifetime,
        applicationHealthState
    )
    {
        _dbContextFactory = dbContextFactory;
        _appSettings = appSettings;
    }

    protected override async Task AdditionalStartAsync(CancellationToken cancellationToken)
    {
        var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (_appSettings.Value.PRODUCTION_MODE)
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        await base.AdditionalStartAsync(cancellationToken);
    }

}
