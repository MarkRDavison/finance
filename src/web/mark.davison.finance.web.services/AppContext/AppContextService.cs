namespace mark.davison.finance.web.services.AppContext;

public class AppContextService : IAppContextService
{
    private readonly IDateService _dateService;

    public AppContextService(IDateService dateService)
    {
        _dateService = dateService;

        (RangeStart, RangeEnd) = _dateService.Today.GetMonthRange();
    }

    public DateOnly RangeStart { get; set; }
    public DateOnly RangeEnd { get; set; }
}
