namespace mark.davison.finance.web.ui.tests.Configuration;

public class AppSettings
{
    public string Section => "FINANCE";

    public AuthSettings AUTH { get; set; } = new();
    public EnvironmentSettings ENVIRONMENT { get; set; } = new();

    public void EnsureValid()
    {
        AUTH.EnsureValid();
        ENVIRONMENT.EnsureValid();
    }
}
