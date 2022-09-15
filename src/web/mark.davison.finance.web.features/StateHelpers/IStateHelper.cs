namespace mark.davison.finance.web.features.StateHelpers;

public interface IStateHelper
{
    IDisposable Force();

    Task FetchAccountList(bool showActive);
    Task FetchCategoryList();
    Task FetchAccountInformation(Guid accountId);

    TimeSpan DefaultReftechTimeSpan { get; }
}
