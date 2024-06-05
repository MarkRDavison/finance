namespace mark.davison.finance.web.components.Components.AppContext;

public partial class AppContext
{
    [Inject]
    public required IAppContextService AppContextService { get; set; }

    [Inject]
    public required IAuthenticationContext AuthenticationContext { get; set; }

    [Inject]
    public required IClientHttpRepository ClientHttpRepository { get; set; }

    [AllowNull]
    private MudDateRangePicker _picker;

    private readonly DateRange _range = new();
    private bool _changeInProgress;

    protected override void OnParametersSet()
    {
        _range.Start = AppContextService.State.RangeStart.ToDateTime(TimeOnly.MinValue);
        _range.End = AppContextService.State.RangeEnd.ToDateTime(TimeOnly.MinValue);
    }

    private void OpenPicker()
    {
        _picker.Open();
    }

    private async Task Reset()
    {
        _picker.Close(false);
        var (s, e) = DateRules.GetMonthRange(DateOnly.FromDateTime(DateTime.Today));
        await DateRangeChanged(new DateRange(s.ToDateTime(TimeOnly.MinValue), e.ToDateTime(TimeOnly.MinValue)));
    }

    private async Task DateRangeChanged(DateRange range)
    {
        if (!_changeInProgress)
        {
            _changeInProgress = true;

            try
            {
                var request = new SetUserContextCommandRequest
                {
                    UserContext = new UserContextDto
                    {
                        StartRange = DateOnly.FromDateTime(range.Start!.Value),
                        EndRange = DateOnly.FromDateTime(range.End!.Value)
                    }
                };

                var response = await ClientHttpRepository.Post<SetUserContextCommandResponse, SetUserContextCommandRequest>(request, CancellationToken.None);

                if (response.SuccessWithValue)
                {
                    _range.Start = response.Value.StartRange.ToDateTime(TimeOnly.MinValue);
                    _range.End = response.Value.EndRange.ToDateTime(TimeOnly.MinValue);

                    AppContextService.UpdateRange(response.Value.StartRange, response.Value.EndRange);
                }
            }
            finally
            {
                _changeInProgress = false;
            }

            await InvokeAsync(StateHasChanged);
        }
    }

    private string FormattedRange => $"{DateOnly.FromDateTime(_range.Start!.Value).ToOrdinalShortDate()} - {DateOnly.FromDateTime(_range.End!.Value).ToOrdinalShortDate()}";

}
