using mark.davison.finance.api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls(urls: Environment.GetEnvironmentVariable("ZENO_FINANCE_URL") ?? "https://0.0.0.0:50000");
            })
            .ConfigureAppConfiguration((hostingContext, configurationBuilder) => {
                configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configurationBuilder.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true);
                configurationBuilder.AddEnvironmentVariables();
            });
}






//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddLogging();
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddLogging();

//builder.Services.AddCors(options =>
//    options.AddPolicy("AllowOrigin", _ => _
//        .SetIsOriginAllowedToAllowWildcardSubdomains()
//        .SetIsOriginAllowed(_ => true)
//        .AllowAnyMethod()
//        .AllowCredentials()
//        .AllowAnyHeader()
//        .Build()
//    ));

//builder.Services.AddDbContextFactory<FinanceDbContext>(_ => 
//    _.UseSqlite($"Data Source={Guid.NewGuid()}.db"));

//builder.Services.AddTransient<IRepository>(_ => 
//    new FinanceRepository(
//        _.GetRequiredService<IDbContextFactory<FinanceDbContext>>(),
//        _.GetRequiredService<ILogger<FinanceRepository>>())
//    );

//builder.Services.AddTransient<IEntityDefaulter<User>, UserDefaulter>();

//var app = builder.Build();
//app.UseCors("AllowOrigin");

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.MapGet("/api/test", () =>
//{
//    return "API says ok!";
//});

//app.Run();
