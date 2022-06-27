var bffRoot = "https://localhost:40000";
var authConfig = new AuthenticationConfig
{
    LoginEndpoint = bffRoot + "/auth/login",
    LogoutEndpoint = bffRoot + "/auth/logout",
    UserEndpoint = bffRoot + "/auth/user"
};

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("API").AddHttpMessageHandler(_ => new CookieHandler());
builder.Services.AddSingleton<IAuthenticationConfig>(authConfig);
builder.Services.AddSingleton<IAuthenticationContext, AuthenticationContext>();
builder.Services.AddSingleton<IClientHttpRepository>(_ => new ClientHttpRepository(bffRoot, _.GetRequiredService<IHttpClientFactory>()));

await builder.Build().RunAsync();
