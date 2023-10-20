using mark.davison.finance.web.components.CommonCandidates.Form.Example;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace mark.davison.finance.web.components.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseFinanceComponents(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddTransient<EditAccountModalViewModel>();
        services.AddTransient<ExampleModalViewModel>();
        return services;
    }
}
