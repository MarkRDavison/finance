using mark.davison.common.client.abstractions.Authentication;
using mark.davison.common.client.abstractions.Repository;

var bffRoot = "https://localhost:40000";
var authConfig = new AuthenticationConfig();
authConfig.SetBffBase(bffRoot);

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("API").AddHttpMessageHandler(_ => new CookieHandler());
builder.Services.AddSingleton<IAuthenticationConfig>(authConfig);
builder.Services.AddSingleton<IAuthenticationContext, AuthenticationContext>();
builder.Services.AddSingleton<IClientHttpRepository>(_ => new FinanceClientHttpRepository(_.GetRequiredService<IAuthenticationConfig>().BffBase, _.GetRequiredService<IHttpClientFactory>()));
builder.Services.UseState();
builder.Services.UseCQRS(typeof(Program), typeof(FeaturesRootType));
builder.Services.AddSingleton<AddAccountViewModel>();

await builder.Build().RunAsync();
