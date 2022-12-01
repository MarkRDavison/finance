namespace mark.davison.finance.web.features.StateHelpers;

public interface IStateHelper
{
    IDisposable Force();

    Task FetchAccountList(bool showActive);
    Task FetchCategoryList();
    Task FetchTagList();
    Task FetchAccountInformation(Guid accountId);

    Task FetchAccountTypeDashboardSummaryData(params Guid[] accountTypeIds);

    TimeSpan DefaultRefetchTimeSpan { get; }
}
