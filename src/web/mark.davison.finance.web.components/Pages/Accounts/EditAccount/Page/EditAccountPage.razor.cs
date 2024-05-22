using Microsoft.AspNetCore.Components.Forms;

namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount.Page;

// TODO: Common base class between edit transaction/account
public partial class EditAccountPage
{
    [Inject, EditorRequired]
    public required IState<StartupState> StartupState { get; set; }

    private EditContext? _editContext;
    private bool _inProgress;
    public bool _primaryDisabled => _inProgress || !FormViewModel.Valid;

    [Parameter, EditorRequired]
    public required Guid Type { get; set; }

    public EditAccountFormViewModel FormViewModel { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        FormViewModel = new()
        {
            AccountTypeId = Type,
            HideAccountType = true
        };

        if (_editContext != null)
        {
            _editContext.OnFieldChanged -= FieldChanged;
        }

        _editContext = new EditContext(FormViewModel);
        _editContext.OnFieldChanged += FieldChanged;

        return EnsureStateLoaded();
    }

    private void FieldChanged(object? sender, FieldChangedEventArgs args) => InvokeAsync(StateHasChanged);

    protected override Task OnParametersSetAsync() => EnsureStateLoaded();

    private Task EnsureStateLoaded() => Task.CompletedTask;

    private async Task OnCreate()
    {
        _inProgress = true;
        if (FormViewModel.Valid)
        {
            var response = await _formSubmission.Primary(FormViewModel);
            if (response.Success)
            {
                _navigation.NavigateTo(RouteHelpers.Account(FormViewModel.Id));
            }
        }
        _inProgress = false;
    }
}
