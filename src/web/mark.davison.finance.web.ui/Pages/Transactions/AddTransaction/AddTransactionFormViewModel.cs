namespace mark.davison.finance.web.ui.Pages.Transactions.AddTransaction;

public class AddTransactionFormViewModel : FormViewModel<AddTransactionFormModel>
{
    public override Task<AddTransactionFormModel> InitialiseModel()
    {
        return Task.FromResult(new AddTransactionFormModel());
    }
}
