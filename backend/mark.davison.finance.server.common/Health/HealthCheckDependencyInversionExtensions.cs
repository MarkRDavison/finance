namespace mark.davison.finance.common.server.Health;

public static class HealthCheckDependencyInversionExtensions
{
    public static void ConfigureHealthCheckServices(this IServiceCollection services)
    {
        services.ConfigureHealthCheckServices<GenericApplicationHealthStateHostedService>();
    }

    public static void ConfigureHealthCheckServices<THealthHosted>(this IServiceCollection services)
        where THealthHosted : class, IApplicationHealthStateHostedService
    {
        services.AddSingleton<IApplicationHealthState, ApplicationHealthState>();

        services.AddHealthChecks()
            .AddCheck<StartupHealthCheck>(StartupHealthCheck.Name)
            .AddCheck<LiveHealthCheck>(LiveHealthCheck.Name)
            .AddCheck<ReadyHealthCheck>(ReadyHealthCheck.Name);

        services.AddHostedService<THealthHosted>();
    }

    public static void MapHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapHealthChecks("/health/startup", new HealthCheckOptions
            {
                Predicate = r => r.Name == StartupHealthCheck.Name
            })
            .AllowAnonymous();
        endpoints
            .MapHealthChecks("/health/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name == LiveHealthCheck.Name
            })
            .AllowAnonymous();
        endpoints
            .MapHealthChecks("/health/readiness", new HealthCheckOptions
            {
                Predicate = r => r.Name == ReadyHealthCheck.Name
            })
            .AllowAnonymous();
    }
}

