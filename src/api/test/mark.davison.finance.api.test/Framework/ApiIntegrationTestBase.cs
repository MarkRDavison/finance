using mark.davison.finance.persistence.Context;

namespace mark.davison.finance.api.test.Framework;

public class ApiIntegrationTestBase : IntegrationTestBase<FinanceApiWebApplicationFactory, AppSettings>
{
    public ApiIntegrationTestBase()
    {
        _factory.ModifyCurrentUserContext = (serviceProvider, currentUserContext) =>
        {
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
            currentUserContext.CurrentUser = CurrentUser;
            currentUserContext.Token = MockJwtTokens.GenerateJwtToken(new[]
            {
                new Claim("sub", CurrentUser.Sub.ToString()),
                new Claim("aud", appSettings.Value.AUTH.CLIENT_ID)
            });
        };
    }

    protected override async Task SeedData(IServiceProvider serviceProvider)
    {
        await base.SeedData(serviceProvider);

        using var scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IFinanceDbContext>();

        await dbContext.UpsertEntityAsync(CurrentUser, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await SeedTestData();
    }

    protected virtual Task SeedTestData() => Task.CompletedTask;

    protected User CurrentUser { get; } = new User
    {
        Id = Guid.NewGuid(),
        Sub = Guid.NewGuid(),
        Username = "integration.test",
        First = "integration",
        Last = "test",
        Email = "integration.test@gmail.com"
    };
}
