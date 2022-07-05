namespace mark.davison.finance.api.Configuration;

public class AppSettings
{
    public static string SECTION = "FINANCE";

    public string AUTHORITY { get; set; } = "https://auth.markdavison.kiwi/auth/realms/markdavison.kiwi";
    public string CLIENT_ID { get; set; } = "zeno-finance";
    public string CLIENT_SECRET { get; set; } = string.Empty;
    public string SESSION_NAME { get; set; } = "finance-session-name";
    public string SCOPE { get; set; } = "openid profile email offline_access zeno zeno-finance";
    public string CONNECTION_STRING { get; set; } = "Data Source=C:/temp/finance-current.db";
}
