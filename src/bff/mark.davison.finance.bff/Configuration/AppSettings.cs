namespace mark.davison.finance.bff.Configuration;

public class AppSettings
{
    public static string SECTION = "FINANCE";

    public string AUTHORITY { get; set; } = "https://auth.markdavison.kiwi/auth/realms/markdavison.kiwi";
    public string CLIENT_ID { get; set; } = "zeno-finance";
    public string CLIENT_SECRET { get; set; } = string.Empty;
    public string SESSION_NAME { get; set; } = "finance-session-name";
    public string SCOPE { get; set; } = "openid profile email offline_access zeno-finance";
    public string REDIS_HOST { get; set; } = "redis.markdavison.kiwi";
    public int REDIS_PORT { get; set; } = 6379;
    public string REDIS_PASSWORD { get; set; } = string.Empty;
    public string WEB_ORIGIN { get; set; } = "https://localhost:8080";
    public string BFF_ORIGIN { get; set; } = "https://localhost:40000";
    public string API_ORIGIN { get; set; } = "https://localhost:50000";
    public List<string> SCOPES => SCOPE.Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(_ => _ != null).ToList();
    public bool PRODUCTION_MODE { get; set; } = false;
}
