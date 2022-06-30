namespace mark.davison.finance.web.ui.Pages.Accounts.AddAccount;

public partial class AddAccountViewModel : ObservableObject
{

    private readonly IMediator _mediator;

    public AddAccountViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ObservableProperty]
    private AddAccountFormViewModel _addAccountFormViewModel = new();

    public async Task<bool> OnSave()
    {
        if (!AddAccountFormViewModel.Valid)
        {
            return false;
        }

        var response = await _mediator.Send(new CreateAccountAction());

        return response.Success;
    }

    public async Task OnCancel()
    {
        await Task.CompletedTask;
    }
}
