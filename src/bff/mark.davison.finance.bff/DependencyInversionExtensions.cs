namespace mark.davison.finance.bff;

public static class DependencyInversionExtensions
{
    public static IServiceCollection UseFinanceBff(this IServiceCollection services, AppSettings appSettings)
    {
        return services.UseFinanceBff(appSettings, null);
    }
    public static IServiceCollection UseFinanceBff(this IServiceCollection services, AppSettings appSettings, Func<HttpClient>? client)
    {
        services.AddScoped<ICurrentUserContext>(_ => new CurrentUserContext());

        services.AddSingleton<IHttpRepository>(_ =>
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new TimeOnlyConverter(),
                    new NullableTimeOnlyConverter(),
                    new DateOnlyConverter(),
                    new NullableDateOnlyConverter()
                }
            };
            if (client == null)
            {
                return new FinanceHttpRepository(appSettings.API_ORIGIN, options);
            }
            return new FinanceHttpRepository(appSettings.API_ORIGIN, client(), options);
        });

        services
            .AddHttpClient()
            .AddHttpContextAccessor();

        services.UseCQRS(
            typeof(BffRootType),
            typeof(CommandsRootType),
            typeof(QueriesRootType),
            typeof(DtosRootType));

        services.AddCommandCQRS();

        return services;
    }
}
