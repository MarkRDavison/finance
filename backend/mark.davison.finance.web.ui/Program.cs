var authConfig = new AuthenticationConfig
{
    LoginEndpoint = "https://localhost:40000/auth/login",
    LogoutEndpoint = "https://localhost:40000/auth/logout",
    UserEndpoint = "https://localhost:40000/auth/user"
};

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("API").AddHttpMessageHandler(_ => new CookieHandler());
builder.Services.AddSingleton<IAuthenticationConfig>(authConfig);
builder.Services.AddSingleton<IAuthenticationContext, AuthenticationContext>();

await builder.Build().RunAsync();
