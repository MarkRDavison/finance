using Fluxor.Blazor.Web.Components;

namespace mark.davison.finance.web.components.Components.Pages;

public abstract class DateRangeWithFluxorComponent : FluxorComponent
{
    [Inject]
    public required IAppContextService AppContextService { get; set; }

    protected AppContextState State { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await ThrottleContextStateLoading();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await ThrottleContextStateLoading();
    }

    protected virtual Task EnsureStateHasLoaded() => Task.CompletedTask;

    protected virtual Task EnsureContextStateHasLoaded() => Task.CompletedTask;

    private async Task ThrottleContextStateLoading()
    {
        if (AppContextService.GetChangedState(State) is AppContextState state)
        {
            State = new AppContextState
            {
                RangeStart = state.RangeStart,
                RangeEnd = state.RangeEnd
            };

            await EnsureContextStateHasLoaded();
            await InvokeAsync(StateHasChanged);
        }
    }
}
