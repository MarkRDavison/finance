namespace mark.davison.finance.common.test.Framework;

public interface IFinanceWebApplicationFactory : IDisposable
{
    public HttpClient CreateClient();
    public Func<IRepository, Task> SeedDataFunc { get; set; }
}

