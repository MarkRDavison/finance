using Microsoft.AspNetCore.Components.Forms;

namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Page;

// TODO: Common base class between edit transaction/account
public partial class EditTransactionPage
{
    private IStateInstance<LookupState> _lookupState { get; set; } = default!;

    private EditContext? _editContext;
    private bool _inProgress;
    public bool _primaryDisabled => _inProgress || !FormViewModel.Valid;

    [Parameter, EditorRequired]
    public required Guid Type { get; set; }

    public EditTransactionFormViewModel FormViewModel { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        _lookupState = GetState<LookupState>();

        FormViewModel = new()
        {
            TransactionTypeId = Type,
            HideTransactionType = true
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
        if (FormViewModel.Valid &&
            await _formSubmission.Primary(FormViewModel))
        {
            _navigation.NavigateTo(RouteHelpers.Transaction(FormViewModel.Id));
        }
        _inProgress = false;
    }
}
