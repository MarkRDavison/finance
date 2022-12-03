namespace mark.davison.finance.bff.commands.test.integration;

public abstract class CQRSIntegrationTestBase : IntegrationTestBase<CQRSFinanceApiWebApplicationFactory, AppSettings>
{
    private IServiceScope? _serviceScope;

    public CQRSIntegrationTestBase()
    {
        _serviceScope = Services.CreateScope();
        _factory.ModifyCurrentUserContext = (serviceProvider, currentUserContext) =>
        {
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
            currentUserContext.CurrentUser = CurrentUser;
            currentUserContext.Token = MockJwtTokens.GenerateJwtToken(new[]
            {
                new Claim("sub", CurrentUser.Sub.ToString()),
                new Claim("aud", appSettings.Value.CLIENT_ID)
            });
        };
    }

    protected User CurrentUser { get; } = new User
    {
        Id = Guid.NewGuid(),
        Sub = Guid.NewGuid(),
        Username = "integration.test",
        First = "integration",
        Last = "test",
        Email = "integration.test@gmail.com"
    };

    protected override async Task SeedData(IServiceProvider serviceProvider)
    {
        await base.SeedData(serviceProvider);
        var repository = serviceProvider.GetRequiredService<IRepository>();
        await repository.UpsertEntityAsync(CurrentUser, CancellationToken.None);
        await SeedTestData();
    }

    protected virtual Task SeedTestData() => Task.CompletedTask;

    [TestCleanup]
    public async Task TestCleanup()
    {
        await Task.CompletedTask;
        _serviceScope?.Dispose();
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        if (_serviceScope == null)
        {
            throw new NullReferenceException();
        }
        return _serviceScope!.ServiceProvider.GetRequiredService<T>();
    }
}
