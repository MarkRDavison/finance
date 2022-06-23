namespace mark.davison.finance.common.test.Framework;

public interface IFinanceWebApplicationFactory<TSettings>
{
    public HttpClient CreateClient();
    public Func<IRepository, Task> SeedDataFunc { get; set; }
}

