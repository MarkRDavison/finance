using mark.davison.finance.common.client;

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
builder.Services.AddSingleton<IClientHttpRepository>(_ => new ClientHttpRepository(bffRoot, _.GetRequiredService<IHttpClientFactory>()));
builder.Services.UseState();
builder.Services.UseCQRS(typeof(Program));
builder.Services.AddSingleton<AddAccountViewModel>();

await builder.Build().RunAsync();
