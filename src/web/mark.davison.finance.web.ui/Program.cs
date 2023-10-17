using mark.davison.finance.web.features.StateHelpers;

var bffRoot = "https://localhost:40000";
var authConfig = new AuthenticationConfig();
authConfig.SetBffBase(bffRoot);

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .UseFinanceComponents()
    .UseFinanceWebServices()
    .UseState()
    .AddSingleton<IAuthenticationConfig>(authConfig)
    .AddSingleton<IAuthenticationContext, AuthenticationContext>()
    .AddSingleton<IStateHelper, StateHelper>()
    .AddSingleton<IClientNavigationManager, ClientNavigationManager>()
    .AddSingleton<IClientHttpRepository>(_ => new FinanceClientHttpRepository(_.GetRequiredService<IAuthenticationConfig>().BffBase, _.GetRequiredService<IHttpClientFactory>()))
    .UseCQRS(typeof(Program), typeof(FeaturesRootType));

builder.Services
    .AddHttpClient(WebConstants.ApiClientName)
    .AddHttpMessageHandler(_ => new CookieHandler());


await builder.Build().RunAsync();
