namespace mark.davison.finance.web.ui.test;

public class AppSettings
{
    public string WebUrl => Environment.GetEnvironmentVariable("WEB_URL") ?? throw new InvalidOperationException("WEB_URL is not present");
    public string BffUrl => Environment.GetEnvironmentVariable("BFF_URL") ?? throw new InvalidOperationException("BFF_URL is not present");
    public string Username => Environment.GetEnvironmentVariable("KC_USERNAME") ?? throw new InvalidOperationException("KC_USERNAME is not present");
    public string Password => Environment.GetEnvironmentVariable("KC_PASSWORD") ?? throw new InvalidOperationException("KC_PASSWORD is not present");
}
