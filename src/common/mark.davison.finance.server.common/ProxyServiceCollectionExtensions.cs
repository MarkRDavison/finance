namespace mark.davison.finance.common.server;

public static class ProxyServiceCollectionExtensions
{
    public static IServiceCollection AddProxyServices(this IServiceCollection services, ProxyOptions options)
    {
        services.AddHttpClient(options.RouteBase, _ =>
        {
            _.BaseAddress = new Uri(options.ProxyAddress);
        });
        return services;
    }
}
