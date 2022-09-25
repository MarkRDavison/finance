using System.Text.Json.Serialization;

namespace mark.davison.finance.api;

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
                AppSettings.CONNECTION_STRING = $"Data Source={Guid.NewGuid()}.db";
            }
            services.AddDbContextFactory<FinanceDbContext>(_ =>
            {
                _.UseSqlite(
                    AppSettings.CONNECTION_STRING,
                    _ => _.MigrationsAssembly("mark.davison.finance.migrations.sqlite"));
                _.EnableSensitiveDataLogging();
                _.EnableDetailedErrors();
            });
        }
        else if (AppSettings.DATABASE_TYPE == "postgres")
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // TODO: Fix/remove when dotnet 7 comes out and we persist dateonly
            var conn = new NpgsqlConnectionStringBuilder();
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

        services.AddTransient<IRepository>(_ =>
            new FinanceRepository(
                _.GetRequiredService<IDbContextFactory<FinanceDbContext>>(),
                _.GetRequiredService<ILogger<FinanceRepository>>())
            );

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
                .MapControllers();
        });

    }

}
