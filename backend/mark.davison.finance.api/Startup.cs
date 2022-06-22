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

        services.AddDbContextFactory<FinanceDbContext>(_ =>
            _.UseSqlite($"Data Source=C:/temp/finance-current.db"));

        services.AddTransient<IFinanceDataSeeder, FinanceDataSeeder>();

        services.AddTransient<IRepository>(_ =>
            new FinanceRepository(
                _.GetRequiredService<IDbContextFactory<FinanceDbContext>>(),
                _.GetRequiredService<ILogger<FinanceRepository>>())
            );

        services.AddTransient<IEntityDefaulter<User>, UserDefaulter>();
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
