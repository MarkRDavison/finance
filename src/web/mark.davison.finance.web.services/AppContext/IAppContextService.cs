namespace mark.davison.finance.web.services.AppContext;

public interface IAppContextService
{
    AppContextState State { get; }

    void UpdateRange(DateOnly start, DateOnly end);

    event EventHandler RangeUpdated;

    AppContextState? GetChangedState(AppContextState existing);
}
