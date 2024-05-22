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
        Console.WriteLine("EditTransaction.FieldChanged: {0}", args.FieldIdentifier.FieldName);
        await InvokeAsync(StateHasChanged);
    }
    protected override Task OnParametersSetAsync() => EnsureStateLoaded();

    private Task EnsureStateLoaded() => Task.CompletedTask;

    private async Task OnCreate()
    {
        _inProgress = true;
        if (FormViewModel.Valid)
        {
            Console.WriteLine("EditTransactionPage.OnCreate 1 - {0}", FormViewModel.Id);
            var response = await _formSubmission.Primary(FormViewModel);
            Console.WriteLine("EditTransactionPage.OnCreate 2 - {0}", FormViewModel.Id);
            if (response.Success)
            {
                Console.WriteLine("EditTransactionPage.OnCreate 2.5");
                _navigation.NavigateTo(RouteHelpers.Transaction(FormViewModel.Id));
            }
        }

        _inProgress = false;
        await InvokeAsync(StateHasChanged);
        Console.WriteLine("EditTransactionPage.OnCreate 2.5");
    }
}
