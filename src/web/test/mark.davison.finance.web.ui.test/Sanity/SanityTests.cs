namespace mark.davison.finance.web.ui.test.Sanity;

[TestClass]
[TestCategory("UI")]
public class SanityTests
{
    private readonly AppSettings appSettings = new();

    [TestMethod]
    public async Task CanLogIn0()
    {
        //const bool dev = true;
        //using var playwright = await Playwright.CreateAsync();
        //await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        //{
        //    Devtools = dev
        //});
        //var page = await browser.NewPageAsync();
        //await page.GotoAsync("https://google.com");
        //await page.GotoAsync("https://auth.markdavison.kiwi");
        //await page.GotoAsync("https://playwright.dev/dotnet");
        //await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png" });
        //// await page.PauseAsync();
        //// pwsh .\bin\Debug\net6.0\playwright.ps1 codegen wikipedia.org
        ///
        Console.WriteLine("Starting CanLogIn0Test");
        await Task.CompletedTask;
        Console.WriteLine("Finishing CanLogIn0Test");

    }

    [TestMethod]
    public async Task CanLogIn1()
    {
        //const bool dev = true;
        //using var playwright = await Playwright.CreateAsync();
        //await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        //{
        //    Devtools = dev
        //});
        //var page = await browser.NewPageAsync();
        //await page.GotoAsync("https://google.com");
        //await page.GotoAsync("https://auth.markdavison.kiwi");
        //await page.GotoAsync("https://playwright.dev/dotnet");
        //await page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png" });
        //// await page.PauseAsync();
        //// pwsh .\bin\Debug\net6.0\playwright.ps1 codegen wikipedia.org
        ///
        Console.WriteLine("Starting CanLogIn1Test");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine("Finishing CanLogIn1Test");

    }

    [TestMethod]
    public async Task CanLogIn2()
    {
        Console.WriteLine("Starting CanLogIn2Test");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine("Finishing CanLogIn2Test");

    }
}
