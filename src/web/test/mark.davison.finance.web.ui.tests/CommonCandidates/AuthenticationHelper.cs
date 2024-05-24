namespace mark.davison.finance.web.ui.tests.CommonCandidates;

public partial class AuthenticationHelper
{
    private readonly AppSettings _appSettings;

    private const string UsernameLabel = "Username or email";
    private const string Password = "Password";
    private const string ExpectedTitle = "Sign in";
    private const string AppTitle = "Zeno Finance";

    public AuthenticationHelper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public async Task EnsureLoggedIn(IPage page)
    {
        var username = string.Empty;

        try
        {
            username = await page.GetByTestId(DataTestIds.Username).TextContentAsync(new LocatorTextContentOptions
            {
                Timeout = 2000.0f
            });
        }
        catch (TimeoutException)
        {

        }

        if (string.IsNullOrEmpty(username))
        {
            bool requiresLogin;
            try
            {
                await Assertions.Expect(page).ToHaveTitleAsync(ExpectedTitleRegex(), new PageAssertionsToHaveTitleOptions
                {
                    Timeout = 2000.0f
                });
                requiresLogin = true;
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (PlaywrightException e)
            {
                if (!e.Message.Contains($"Page title expected to be '{ExpectedTitle}'") ||
                    !e.Message.Contains($"But was: '{AppTitle}'"))
                {
                    throw;
                }
                requiresLogin = false;
            }

            if (requiresLogin)
            {
                await page.GetByLabel(UsernameLabel).FillAsync(_appSettings.AUTH.USERNAME);
                await page.GetByLabel(Password).FillAsync(_appSettings.AUTH.PASSWORD);

                var button = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
                {
                    NameString = ButtonNames.SignIn
                });

                await button.ClickAsync();
            }
        }

        await Assertions.Expect(page).ToHaveURLAsync(new Regex(_appSettings.ENVIRONMENT.WEB_ORIGIN));
        await Assertions.Expect(page).ToHaveTitleAsync(AppTitle);
    }

    [GeneratedRegex(ExpectedTitle, RegexOptions.Compiled)]
    private static partial Regex ExpectedTitleRegex();
}