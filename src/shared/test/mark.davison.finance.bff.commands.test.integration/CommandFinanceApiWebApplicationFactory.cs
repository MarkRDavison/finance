namespace mark.davison.finance.bff.commands.test.integration;

public class CommandFinanceApiWebApplicationFactory : FinanceApiWebApplicationFactory
{
    public CommandFinanceApiWebApplicationFactory()
    {

    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var config = new OpenIdConnectConfiguration()
            {
                Issuer = MockJwtTokens.Issuer
            };

            config.SigningKeys.Add(MockJwtTokens.SecurityKey);
            options.Configuration = config;
        });
        services.UseFinanceBff(new()
        {
            API_ORIGIN = "http://localhost/"
        }, CreateClient);
    }
}
