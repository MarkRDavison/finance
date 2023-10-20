using mark.davison.common.server.Endpoints;
using mark.davison.common.Services;
using mark.davison.common.source.generators.CQRS;
using mark.davison.finance.bff.commands;
using mark.davison.finance.bff.queries;
using mark.davison.finance.models.dtos;

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
        var AppSettings = services.ConfigureSettingsServices(Configuration);
        if (AppSettings == null) { throw new InvalidOperationException(); }

        services.AddLogging();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = AppSettings.AUTHORITY;
                o.Audience = AppSettings.CLIENT_ID;
            });

        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        services.AddControllers().AddJsonOptions(_ =>
        {
            _.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.ConfigureHealthCheckServices<InitializationHostedService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(options =>
            options.AddPolicy("AllowOrigin", _ => _
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader()
                .Build()
            ));

        if (AppSettings.DATABASE_TYPE == "sqlite")
        {
            if (AppSettings.CONNECTION_STRING.Equals("RANDOM", StringComparison.OrdinalIgnoreCase))
            {
                if (Directory.Exists("C:/temp"))
                {
                    AppSettings.CONNECTION_STRING = $"Data Source=C:/temp/{Guid.NewGuid()}.db";
                }
                else
                {
                    AppSettings.CONNECTION_STRING = $"Data Source={Guid.NewGuid()}.db";
                }
            }
            services.AddDbContextFactory<FinanceDbContext>(_ =>
            {
                _.UseSqlite(
                    AppSettings.CONNECTION_STRING,
                    _ => _.MigrationsAssembly("mark.davison.finance.migrations.sqlite"));
                if (!AppSettings.PRODUCTION_MODE)
                {
                    _.EnableSensitiveDataLogging();
                    _.EnableDetailedErrors();
                }
            });
        }
        else if (AppSettings.DATABASE_TYPE == "postgres")
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // TODO: Fix/remove when dotnet 7 comes out and we persist dateonly
            var conn = new NpgsqlConnectionStringBuilder();
            conn.IncludeErrorDetail = !AppSettings.PRODUCTION_MODE;
            conn.Host = AppSettings.DB_HOST;
            conn.Database = AppSettings.DB_DATABASE;
            conn.Port = AppSettings.DB_PORT;
            conn.Username = AppSettings.DB_USERNAME;
            conn.Password = AppSettings.DB_PASSWORD;
            services.AddDbContextFactory<FinanceDbContext>(_ => _
                .UseNpgsql(
                    conn.ConnectionString,
                    _ => _.MigrationsAssembly("mark.davison.finance.migrations.postgresql")));
        }
        else
        {
            throw new NotImplementedException($"Cannot handle this database type: {AppSettings.DATABASE_TYPE}");
        }

        services.AddTransient<IFinanceDataSeeder, FinanceDataSeeder>();
        services.AddSingleton<IDateService>(new DateService(DateService.DateMode.Utc));

        services.AddScoped<IRepository>(_ =>
            new FinanceRepository(
                _.GetRequiredService<IDbContextFactory<FinanceDbContext>>(),
                _.GetRequiredService<ILogger<FinanceRepository>>())
            );


        services
            .AddHttpClient()
            .AddHttpContextAccessor();
        services.AddCommandCQRS();
        services.UseCQRSServer();
        services.UseFinancePersistence();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors("AllowOrigin");

        app.UseHttpsRedirection();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<RequestResponseLoggingMiddleware>();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<HydrateAuthenticationFromClaimsPrincipalMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints
                .MapHealthChecks();

            endpoints
                .ConfigureCQRSEndpoints();

            endpoints
                .UseGet<User>()
                .UseGetById<User>()
                .UsePost<User>();

            //.UseGet<Account>()
            //.UseGetById<Account>()
            //.UsePost<Account>()

            //.UseGet<AccountType>()
            //.UseGetById<AccountType>()
            //.UsePost<AccountType>()

            //.UseGet<Category>()
            //.UseGetById<Category>()
            //.UsePost<Category>()

            //.UseGet<Currency>()
            //.UseGetById<Currency>()
            //.UsePost<Currency>()

            //.UseGet<Tag>()
            //.UseGetById<Tag>()
            //.UsePost<Tag>()

            //.UseGet<Transaction>()
            //.UseGetById<Transaction>()
            //.UsePost<Transaction>()

            //.UseGet<TransactionJournal>()
            //.UseGetById<TransactionJournal>()
            //.UsePost<TransactionJournal>()

            //.UseGet<TransactionGroup>()
            //.UseGetById<TransactionGroup>()
            //.UsePost<TransactionGroup>()

            //.UseGet<TransactionType>()
            //.UseGetById<TransactionType>()
            //.UsePost<TransactionType>();
        });

    }

}
