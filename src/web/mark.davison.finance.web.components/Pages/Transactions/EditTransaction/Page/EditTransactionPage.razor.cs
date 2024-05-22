using Microsoft.AspNetCore.Components.Forms;

namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Page;

// TODO: Common base class between edit transaction/account
public partial class EditTransactionPage
{
    [Inject, EditorRequired]
    public required IState<StartupState> StartupState { get; set; }

    private EditContext? _editContext;
    private bool _inProgress;
    public bool _primaryDisabled => _inProgress || !FormViewModel.Valid;

    [Parameter, EditorRequired]
    public required Guid Type { get; set; }

    public EditTransactionFormViewModel FormViewModel { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        FormViewModel = new()
        {
            TransactionTypeId = Type
        };

        FormViewModel.AddSplit();

        if (_editContext != null)
        {
            _editContext.OnFieldChanged -= FieldChanged;
        }

        _editContext = new EditContext(FormViewModel);
        _editContext.OnFieldChanged += FieldChanged;

        return EnsureStateLoaded();
    }
    private async void FieldChanged(object? sender, FieldChangedEventArgs args)
    {
        await InvokeAsync(StateHasChanged);
    }
    //protected override Task OnParametersSetAsync() => EnsureStateLoaded();

    private Task EnsureStateLoaded() => Task.CompletedTask;

    private async Task OnCreate()
    {
        _inProgress = true;
        if (FormViewModel.Valid)
        {
            if (await _formSubmission.Primary(FormViewModel) is { Success: true })
            {
                _navigation.NavigateTo(RouteHelpers.Transaction(FormViewModel.Id));
            }
        }

        _inProgress = false;
        await InvokeAsync(StateHasChanged);
    }
}
