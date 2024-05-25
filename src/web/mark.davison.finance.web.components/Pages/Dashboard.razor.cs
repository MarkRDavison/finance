using ApexCharts;

namespace mark.davison.finance.web.components.Pages;

public sealed class AccountSummaryAmount
{
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
}

public partial class Dashboard
{
    [Inject]
    public required IState<StartupState> StartupState { get; set; }

    [Inject]
    public required IState<AccountState> AccountState { get; set; }

    [Inject]
    public required IState<DashboardState> DashboardState { get; set; }

    [Inject]
    public required IStateHelper StateHelper { get; set; }

    private string AccountNameById(Guid accountId) => AccountState.Value.Accounts.FirstOrDefault(_ => _.Id == accountId)?.Name ?? string.Empty;

    public static ApexChartOptions<AccountSummaryAmount> Options => new()
    {
        Legend = new Legend
        {
            Show = true,
            Position = LegendPosition.Bottom,
            HorizontalAlign = ApexCharts.Align.Center,
            ShowForSingleSeries = true
        },
        Chart = new Chart
        {
            Selection = new Selection
            {
                Enabled = false
            },
            Zoom = new Zoom
            {
                Enabled = false
            },
            Toolbar = new Toolbar
            {
                Tools = new Tools
                {
                    Pan = false,
                    Selection = false
                },
                Show = false
            }
        },
        Xaxis = new XAxis
        {
            TickAmount = 2,
            Type = XAxisType.Datetime,
            Labels = new XAxisLabels
            {
                //                Formatter = @"function (value, timestamp) {
                //return 'some custom text'
                //}"
                //DatetimeFormatter = new DatetimeFormatter
                //{
                //    Year = "yyyy",
                //    Month = "MMM 'yy",
                //    Day = "dd MMM"
                //},
                //Formatter = @"function(value, { series, seriesIndex, dataPointIndex, w }) { return '$' + value }"
            }
        },
        NoData = new NoData
        {
            Text = "Loading"
        },
        Theme = new Theme
        {
            Mode = Mode.Dark // TODO: Need to get mudblazor mode and set here
        }
    };

    protected override async Task OnInitializedAsync()
    {
        await EnsureStateHasLoaded();
    }

    private async Task EnsureStateHasLoaded()
    {
        await StateHelper.FetchAccountList(false);
        await StateHelper.FetchAccountTypeDashboardSummaryData(AccountTypeConstants.Asset);
    }
    private string GetYAxisLabel(decimal value)
    {
        return "$" + value.ToString("N0");
    }
    public static AccountSummaryAmount ToSummary(AccountDashboardTransactionData data) => new AccountSummaryAmount
    {
        Date = data.Date, // TODO: UTC -> LOCAL???
        Amount = CurrencyRules.FromPersisted(data.Amount)
    };
}
