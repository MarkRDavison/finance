namespace mark.davison.finance.web.features.StateHelpers;

public interface IStateHelper
{
    IDisposable Force();

    Task FetchAccountList(bool showActive);

    TimeSpan DefaultReftechTimeSpan { get; }
}
