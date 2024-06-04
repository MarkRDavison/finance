namespace mark.davison.finance.web.components.Pages;

public sealed class AccountSummaryAmount
{
    public DateOnly Date { get; set; }
    public decimal? Amount { get; set; }
}

public partial class Dashboard
{
    [CascadingParameter]
    public required ThemeInfo ThemeInfo { get; set; }

    [Inject]
    public required IState<StartupState> StartupState { get; set; }

    [Inject]
    public required IState<AccountState> AccountState { get; set; }

    [Inject]
    public required IState<DashboardState> DashboardState { get; set; }

    [Inject]
    public required IStateHelper StateHelper { get; set; }

    private string AccountNameById(Guid accountId) => AccountState.Value.Accounts.FirstOrDefault(_ => _.Id == accountId)?.Name ?? string.Empty;

    public ApexChartOptions<AccountSummaryAmount> Options => new()
    {
        Legend = new Legend
        {
            Show = true,
            Position = LegendPosition.Bottom,
            HorizontalAlign = ApexCharts.Align.Center,
            ShowForSingleSeries = true,
            ShowForNullSeries = true
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
            Type = XAxisType.Datetime
        },
        NoData = new NoData
        {
            Text = DashboardState.Value.Loading
                ? "Loading..."
                : "No data"
        },
        Theme = new Theme
        {
            Mode = ThemeInfo.DarkMode ? Mode.Dark : Mode.Light
        }
    };

    protected override async Task EnsureContextStateHasLoaded()
    {
        await StateHelper.FetchAccountTypeDashboardSummaryData(AccountTypeConstants.Asset);
    }

    protected override async Task EnsureStateHasLoaded()
    {
        await StateHelper.FetchAccountList(false);
        await EnsureContextStateHasLoaded();
    }

    private string GetYAxisLabel(decimal value) => value.ToString("N0");

    public static AccountSummaryAmount ToSummary(AccountDashboardTransactionData data) => new AccountSummaryAmount
    {
        Date = data.Date, // TODO: UTC -> LOCAL???
        Amount = CurrencyRules.FromPersisted(data.Amount)
    };
}
