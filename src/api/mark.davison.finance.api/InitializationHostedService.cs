namespace mark.davison.finance.api;

public class InitializationHostedService : GenericApplicationHealthStateHostedService
{
    private readonly IFinanceDataSeeder _financeDataSeeder;

    public InitializationHostedService(
        IHostApplicationLifetime hostApplicationLifetime,
        IApplicationHealthState applicationHealthState,
        IFinanceDataSeeder financeDataSeeder
    ) : base(
        hostApplicationLifetime,
        applicationHealthState
    )
    {
        _financeDataSeeder = financeDataSeeder;
    }

    protected override async Task AdditionalStartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _financeDataSeeder.EnsureDataSeeded(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
        await base.AdditionalStartAsync(cancellationToken);
    }

}
