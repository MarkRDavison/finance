var bffRoot = "https://localhost:40000";
var authConfig = new AuthenticationConfig();
authConfig.SetBffBase(bffRoot);

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient(WebConstants.ApiClientName).AddHttpMessageHandler(_ => new CookieHandler());
builder.Services.AddSingleton<IAuthenticationConfig>(authConfig);
builder.Services.AddSingleton<IAuthenticationContext, AuthenticationContext>();
builder.Services.AddSingleton<IStateHelper, StateHelper>();
builder.Services.AddSingleton<IClientNavigationManager, ClientNavigationManager>();
builder.Services.AddSingleton<IClientHttpRepository>(_ => new FinanceClientHttpRepository(_.GetRequiredService<IAuthenticationConfig>().BffBase, _.GetRequiredService<IHttpClientFactory>()));
builder.Services.UseFinanceWebServices();
builder.Services.UseState();
builder.Services.UseCQRS(typeof(Program), typeof(FeaturesRootType));

// TODO: Interface/pattern to auto register these
builder.Services.AddTransient<EditAccountViewModel>();
builder.Services.AddTransient<AddCategoryModalViewModal>();
builder.Services.AddTransient<AddTransactionPageViewModel>();
builder.Services.AddTransient<AddTagModalViewModel>();

await builder.Build().RunAsync();
