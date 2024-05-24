namespace mark.davison.finance.web.ui.tests.CommonCandidates;

[TestClass]
public abstract class BaseTest : PlaywrightTest, IAsyncDisposable
{
    private IBrowser _browser = default!;
    private IBrowserContext _context = default!;
    private readonly Faker _faker;
    private const string _authStateFilename = ".auth.json";

    protected BaseTest()
    {
        AppSettings = CreateAppSettings();

        AuthenticationHelper = new AuthenticationHelper(AppSettings);

        _faker = new Faker();
    }

    private static AppSettings CreateAppSettings()
    {
        var appSettings = new AppSettings();
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();

        config.GetRequiredSection(appSettings.Section).Bind(appSettings);

        appSettings.EnsureValid();

        return appSettings;
    }

    private static string AuthStateFullPath(string? tempDir) => $"{tempDir?.TrimEnd('/')}/{_authStateFilename}";

    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext _)
    {
        var appSettings = CreateAppSettings();

        if (File.Exists(AuthStateFullPath(appSettings.ENVIRONMENT.TEMP_DIR)))
        {
            File.Delete(AuthStateFullPath(appSettings.ENVIRONMENT.TEMP_DIR));
        }

        using var client = new HttpClient();
        await client.PostAsync($"{appSettings.ENVIRONMENT.API_ORIGIN}/api/reset", null);
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await OnPreTestInitialize();

        _browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = !Debug,
            SlowMo = Debug ? 250 : null
        });

        _context = await _browser.NewContextAsync(new()
        {
            StorageStatePath = File.Exists(AuthStateFullPath(AppSettings.ENVIRONMENT.TEMP_DIR)) ? AuthStateFullPath(AppSettings.ENVIRONMENT.TEMP_DIR) : null
        });

        CurrentPage = await _context.NewPageAsync();

        await CurrentPage.GotoAsync(AppSettings.ENVIRONMENT.WEB_ORIGIN);

        await OnTestInitialise();
    }

    protected virtual Task OnPreTestInitialize() => Task.CompletedTask;

    protected virtual Task OnTestInitialise() => Task.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed && !string.IsNullOrEmpty(AppSettings.ENVIRONMENT.TEMP_DIR))
        {
            await CurrentPage.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = AppSettings.ENVIRONMENT.TEMP_DIR + "screenshot_" + TestContext.TestName + Guid.NewGuid().ToString().Replace("-", "_") + ".png",
                Type = ScreenshotType.Png
            });
        }
        else if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed && !string.IsNullOrEmpty(AppSettings.ENVIRONMENT.TEMP_DIR))
        {
            await _context.StorageStateAsync(new()
            {
                Path = AuthStateFullPath(AppSettings.ENVIRONMENT.TEMP_DIR)
            });
        }

        await _context.DisposeAsync();
        await _browser.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    protected AppSettings AppSettings { get; }
    protected AuthenticationHelper AuthenticationHelper { get; }
    protected DashboardPage Dashboard => new(CurrentPage, AppSettings);
    protected IPage CurrentPage { get; set; } = default!;

    protected virtual bool Debug => Debugger.IsAttached;

    protected string GetSentence(int words = 3) => _faker.Lorem.Sentence(words);
    protected string GetNoun() => _faker.Hacker.Noun();
    protected string MakeAccountNumber() => _faker.Finance.Account();
    protected string MakeAccountName() => _faker.Finance.AccountName();

}
