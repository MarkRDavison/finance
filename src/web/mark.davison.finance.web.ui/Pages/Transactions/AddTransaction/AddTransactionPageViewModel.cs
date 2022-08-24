namespace mark.davison.finance.web.ui.Pages.Transactions.AddTransaction;

public class AddTransactionPageViewModel
{

    private readonly ICQRSDispatcher _dispatcher;
    private readonly IStateStore _stateStore;

    public AddTransactionPageViewModel(
        ICQRSDispatcher dispatcher,
        IStateStore stateStore)
    {
        _dispatcher = dispatcher;
        _stateStore = stateStore;

        AddTransactionFormViewModels.Add(new());
    }

    public Guid TransactionTypeId { get; set; }
    public List<AddTransactionFormViewModel> AddTransactionFormViewModels { get; set; } = new();
    public string SplitTransactionDescription { get; set; } = string.Empty;

    public void AddSplitTransaction()
    {
        AddTransactionFormViewModels.Add(new());
    }

    public void RemoveSplitTransaction(AddTransactionFormViewModel viewModel)
    {
        AddTransactionFormViewModels.Remove(viewModel);
    }

    public async Task Submit()
    {
        var request = new TransactionCreateCommand
        {
            Description = string.Empty,
            TransactionTypeId = TransactionTypeId,
            CreateTransactionDtos = AddTransactionFormViewModels.Select(_ =>
            {
                var sourceTransactionAccount = _stateStore.GetState<AccountListState>().Instance.Accounts.First(__ => __.Id == _.Model.SourceAccountId);
                return new CreateTransactionDto
                {
                    Id = Guid.NewGuid(),
                    Description = _.Model.Description,
                    SourceAccountId = _.Model.SourceAccountId,
                    DestinationAccountId = _.Model.DestinationAccountId,
                    Date = _.Model.Date,
                    Amount = (long)(_.Model.Amount * (decimal)Math.Pow(10, CurrencyRules.FinanceDecimalPlaces)),
                    ForeignAmount = _.Model.ForeignAmount == 0
                        ? null
                        : (long)(_.Model.ForeignAmount * (decimal)Math.Pow(10, CurrencyRules.FinanceDecimalPlaces)),
                    CurrencyId = sourceTransactionAccount.CurrencyId,
                    ForeignCurrencyId = _.Model.ForeignCurrencyId == Guid.Empty ? null : _.Model.ForeignCurrencyId,
                    BillId = null,
                    BudgetId = null,
                    CategoryId = null,
                };
            }).ToList()
        };

        var response = await _dispatcher.Dispatch<TransactionCreateCommand, TransactionCreateCommandResponse>(request, CancellationToken.None);
    }

}
