namespace mark.davison.finance.api.Middleware;

public class LoadFinanceUserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoadFinanceUserContextMiddleware> _logger;

    public LoadFinanceUserContextMiddleware(
        RequestDelegate next,
        ILogger<LoadFinanceUserContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IFinanceUserContext financeUserContext)
    {
        await financeUserContext.LoadAsync(CancellationToken.None);
        _logger.LogInformation("Loaded user context {0} -> {1}", financeUserContext.RangeStart.ToShortDateString(), financeUserContext.RangeEnd.ToShortDateString());
        await _next(context);
    }
}
