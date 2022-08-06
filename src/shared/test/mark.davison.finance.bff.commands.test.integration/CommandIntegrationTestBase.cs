namespace mark.davison.finance.bff.commands.test.integration;

public abstract class CommandIntegrationTestBase : IntegrationTestBase<CommandFinanceApiWebApplicationFactory, AppSettings>
{
    private IServiceScope? _serviceScope;
    protected User CurrentUser { get; } = new User
    {
        Id = Guid.NewGuid(),
        Sub = Guid.NewGuid(),
        Username = "integration.test",
        First = "integration",
        Last = "test",
        Email = "integration.test@gmail.com"
    };
    protected override async Task SeedData(IRepository repository)
    {
        await base.SeedData(repository);
        await repository.UpsertEntityAsync(CurrentUser, CancellationToken.None);
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await Task.CompletedTask;
        _serviceScope = Services.CreateScope();
        var currentUserContext = _serviceScope.ServiceProvider.GetRequiredService<ICurrentUserContext>();
        var appSettings = _serviceScope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>();
        currentUserContext.CurrentUser = CurrentUser;
        currentUserContext.Token = MockJwtTokens.GenerateJwtToken(new[]
        {
            new Claim("sub", CurrentUser.Sub.ToString()),
            new Claim("aud", appSettings.Value.CLIENT_ID)
        });
    }

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
