namespace mark.davison.finance.web.services.AppContext;

public class AppContextService : IAppContextService
{
    private readonly IDateService _dateService;

    public AppContextService(IDateService dateService)
    {
        _dateService = dateService;

        State = new();

        (State.RangeStart, State.RangeEnd) = _dateService.Today.GetMonthRange();
    }

    public AppContextState State { get; }

    public event EventHandler RangeUpdated = default!;

    public void UpdateRange(DateOnly start, DateOnly end)
    {
        State.RangeStart = start;
        State.RangeEnd = end;

        RangeUpdated.Invoke(this, EventArgs.Empty);
    }

    public AppContextState? GetChangedState(AppContextState existing)
    {
        if (State.RangeStart != existing.RangeStart ||
            State.RangeEnd != existing.RangeEnd)
        {
            Console.WriteLine("GetChangedState - changed!");
            return State;
        }

        Console.WriteLine("GetChangedState - not changed");
        Console.WriteLine(" - start: {0} vs {1}", State.RangeStart, existing.RangeStart);
        Console.WriteLine(" - end:   {0} vs {1}", State.RangeEnd, existing.RangeEnd);
        return null;
    }
}
