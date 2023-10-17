using mark.davison.finance.web.components.Pages.Accounts.EditAccount;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace mark.davison.finance.web.components.Ignition;

public static class DependecyInjectionExtensions
{
    public static IServiceCollection UseFinanceComponents(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddTransient<EditAccountViewModel>();
        return services;
    }
}
