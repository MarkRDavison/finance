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

        services.AddControllers();

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
                services.AddDbContextFactory<FinanceDbContext>(_ => _
                    .UseSqlite($"Data Source={Guid.NewGuid()}.db"));
            }
            else
            {
                services.AddDbContextFactory<FinanceDbContext>(_ => _
                    .UseSqlite(AppSettings.CONNECTION_STRING));
            }
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
