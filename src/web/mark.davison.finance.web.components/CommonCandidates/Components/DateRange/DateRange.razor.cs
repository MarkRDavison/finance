namespace mark.davison.finance.web.components.CommonCandidates.Components.DateRange;

public partial class DateRange
{

    private DateTime? _startDateTime;
    private DateTime? _endDateTime;
    private DateTime? MinDateTime => Min?.ToDateTime(TimeOnly.MinValue);
    private DateTime? MaxDateTime => Max?.ToDateTime(TimeOnly.MaxValue);


    [Parameter, EditorRequired]
    public required DateOnly Start { get; set; }

    [Parameter, EditorRequired]
    public required DateOnly End { get; set; }

    [Parameter]
    public EventCallback<DateOnly> StartChanged { get; set; }

    [Parameter]
    public EventCallback<DateOnly> EndChanged { get; set; }

    [Parameter]
    public DateOnly? Min { get; set; }

    [Parameter]
    public DateOnly? Max { get; set; }


    protected override void OnParametersSet()
    {
        _startDateTime = Start.ToDateTime(TimeOnly.MinValue);
        _endDateTime = End.ToDateTime(TimeOnly.MaxValue);
    }

    private async Task StartDateChanged(DateTime? startDate)
    {
        Start = startDate == null ? DateOnly.MinValue : DateOnly.FromDateTime(startDate.Value);
        await StartChanged.InvokeAsync(Start);
    }
    private async Task EndDateChanged(DateTime? endDate)
    {
        End = endDate == null ? DateOnly.MaxValue : DateOnly.FromDateTime(endDate.Value);
        await EndChanged.InvokeAsync(End);
    }
}
