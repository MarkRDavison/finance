using mark.davison.finance.accounting.constants;
using mark.davison.finance.accounting.rules;
using mark.davison.finance.data.helpers.Seeders;
using mark.davison.finance.data.helpers.test.constants;
using mark.davison.finance.models.dtos.Shared;
using mark.davison.finance.models.Entities;
using mark.davison.finance.shared.utilities.Ignition;

namespace mark.davison.finance.api;

[UseCQRSServer(typeof(DtosRootType), typeof(CommandsRootType), typeof(QueriesRootType))]
public class Startup
{
    public IConfiguration Configuration { get; }

    public AppSettings AppSettings { get; set; } = null!;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings = services.ConfigureSettingsServices<AppSettings>(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        Console.WriteLine(AppSettings.DumpAppSettings(AppSettings.PRODUCTION_MODE));

        services
            .AddCors()
            .AddLogging()
            .AddJwtAuth(AppSettings.AUTH)
            .AddAuthorization()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserContext, CurrentUserContext>()
            .AddHealthCheckServices<InitializationHostedService>()
            .AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc))
            .AddDatabase<FinanceDbContext>(AppSettings.PRODUCTION_MODE, AppSettings.DATABASE, typeof(PostgresContextFactory), typeof(SqliteContextFactory))
            .AddCoreDbContext<FinanceDbContext>()
            .AddScoped<IFinanceDbContext>(_ => _.GetRequiredService<FinanceDbContext>())
            .AddCQRSServer()
            .AddRedis(AppSettings.REDIS, AppSettings.SECTION, AppSettings.PRODUCTION_MODE)
            .UseFinancePersistence() // TODO: Include optional defaulters in IFinanceDbContext wrapper????
            .AddSharedServices()
            .AddCommandCQRS()
            .UseDataSeeders();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
            builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(_ => true) // TODO: Config driven
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader());

        app.UseHttpsRedirection();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app
            .UseMiddleware<RequestResponseLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseMiddleware<PopulateUserContextMiddleware>()
            .UseMiddleware<ValidateUserExistsInDbMiddleware>()
            .UseEndpoints(endpoints =>
            {
                endpoints
                    .MapHealthChecks()
                    .MapGet<User>()
                    .MapGetById<User>()
                    .MapPost<User>()
                    .MapCQRSEndpoints();

                if (!AppSettings.PRODUCTION_MODE)
                {
                    endpoints
                        .MapPost("/api/seed", async (HttpContext context) =>
                        {
                            var today = DateOnly.FromDateTime(DateTime.Today);
                            var monthStart = new DateOnly(today.Year, today.Month, 1);

                            var accountSeeder = context.RequestServices.GetRequiredService<AccountSeeder>();
                            var transactionSeeder = context.RequestServices.GetRequiredService<TransactionSeeder>();

                            await accountSeeder.CreateStandardAccounts();
                            await transactionSeeder.CreateTransaction(new()
                            {
                                TransactionTypeId = TransactionTypeConstants.Deposit,
                                Transactions = [
                                    new CreateTransactionDto
                                    {
                                        Id = Guid.NewGuid(),
                                        Amount = CurrencyRules.ToPersisted(100.0M),
                                        CurrencyId = Currency.NZD,
                                        SourceAccountId =AccountTestConstants.RevenueAccount1Id,
                                        DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                                        Date = monthStart
                                    }
                                ]
                            });
                            await transactionSeeder.CreateTransaction(new()
                            {
                                TransactionTypeId = TransactionTypeConstants.Deposit,
                                Transactions = [
                                    new CreateTransactionDto
                                    {
                                        Id = Guid.NewGuid(),
                                        Amount = CurrencyRules.ToPersisted(80.0M),
                                        CurrencyId = Currency.NZD,
                                        SourceAccountId =AccountTestConstants.RevenueAccount1Id,
                                        DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                                        Date = monthStart.AddDays(2)
                                    }
                                ]
                            });
                            await transactionSeeder.CreateTransaction(new()
                            {
                                TransactionTypeId = TransactionTypeConstants.Deposit,
                                Transactions = [
                                    new CreateTransactionDto
                                    {
                                        Id = Guid.NewGuid(),
                                        Amount = CurrencyRules.ToPersisted(300.0M),
                                        CurrencyId = Currency.NZD,
                                        SourceAccountId =AccountTestConstants.RevenueAccount2Id,
                                        DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                                        Date = monthStart.AddDays(5)
                                    }
                                ]
                            });

                            return Results.Ok();
                        });

                    endpoints
                        .MapPost("/api/reset", async (HttpContext context, CancellationToken cancellationToken) =>
                        {
                            var dbContext = context.RequestServices.GetRequiredService<IFinanceDbContext>();

                            var users = await dbContext
                                .Set<User>()
                                .Where(_ => _.Id != Guid.Empty)
                                .ToListAsync(cancellationToken);
                            await dbContext.DeleteEntitiesAsync(users, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);

                            return Results.Ok();
                        }).AllowAnonymous();
                }
            });

    }

}
