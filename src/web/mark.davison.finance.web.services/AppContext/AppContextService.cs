namespace mark.davison.finance.web.services.AppContext;

public class AppContextService : IAppContextService
{
    public AppContextService()
    {
        RangeStart = new DateOnly(DateOnly.FromDateTime(DateTime.Today).Year, DateOnly.FromDateTime(DateTime.Today).Month, 1);
        RangeEnd = new DateOnly(DateOnly.FromDateTime(DateTime.Today).Year, DateOnly.FromDateTime(DateTime.Today).Month, 1); // TODO: This doesn't work properly
    }

    public DateOnly RangeStart { get; set; }
    public DateOnly RangeEnd { get; set; }
}
