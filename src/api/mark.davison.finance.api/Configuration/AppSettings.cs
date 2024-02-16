namespace mark.davison.finance.api.Configuration;

public class AppSettings
{
    public static string SECTION = "FINANCE";

    public string AUTHORITY { get; set; } = "https://keycloak.markdavison.kiwi/auth/realms/markdavison.kiwi";
    public string CLIENT_ID { get; set; } = "zeno-finance";
    public string CLIENT_SECRET { get; set; } = string.Empty;
    public string SESSION_NAME { get; set; } = "finance-session-name";
    public string SCOPE { get; set; } = "openid profile email offline_access zeno-finance";
    public string CONNECTION_STRING { get; set; } = "Data Source=C:/temp/finance-current.db";
    public string DATABASE_TYPE { get; set; } = "sqlite";
    public bool PRODUCTION_MODE { get; set; } = false;
    public int DB_PORT { get; set; } = 5432;
    public string DB_HOST { get; set; } = "localhost";
    public string DB_DATABASE { get; set; } = "finance-dev";
    public string DB_USERNAME { get; set; } = string.Empty;
    public string DB_PASSWORD { get; set; } = string.Empty;
    public string REDIS_HOST { get; set; } = "redis.markdavison.kiwi";
    public int REDIS_PORT { get; set; } = 6379;
    public string REDIS_PASSWORD { get; set; } = string.Empty;
}
