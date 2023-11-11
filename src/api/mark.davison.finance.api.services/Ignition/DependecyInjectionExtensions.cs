namespace mark.davison.finance.api.services.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseUserApplicationContext(this IServiceCollection services)
    {
        services.AddScoped<IUserApplicationContext>(_ =>
        {
            return new UserApplicationContext.UserApplicationContext(
                _.GetRequiredService<IDistributedCache>(),
                _.GetRequiredService<ICurrentUserContext>(),
                _.GetRequiredService<IDateService>()
            );
        });
        return services;
    }
}
