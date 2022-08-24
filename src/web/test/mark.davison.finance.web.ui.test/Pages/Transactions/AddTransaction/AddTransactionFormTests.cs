namespace mark.davison.finance.web.ui.test.Pages.Transactions.AddTransaction;

[TestClass]
public class AddTransactionFormTests : TestBase
{
    private readonly AddTransactionFormViewModel _viewModel;

    public AddTransactionFormTests()
    {
        _viewModel = new AddTransactionFormViewModel();
    }

    protected override void SetupTest()
    {
        base.SetupTest();
        Services.Add(new ServiceDescriptor(typeof(AddTransactionFormViewModel), _viewModel));
    }

    [TestMethod]
    public void InitialForm_RendersWithExpectedFields()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        var inputs = cut.FindAll(".z-form-control");
        Assert.AreEqual(7, inputs.Count());
    }

    [TestMethod]
    public void UpdatingDescription_UpdatesModel()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        var descriptionText = "some description";
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetTextInputValueByLabel("Description", descriptionText);
        Assert.AreEqual(descriptionText, _viewModel.Model.Description);
    }

    [TestMethod]
    public void UpdatingSourceAccount_UpdatesModel()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        var sourceAccount = new AccountListItemDto
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountConstants.Revenue,
            Name = "Test revenue account"
        };
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState(
            sourceAccount));
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetDropdownValueByLabel("Source account", sourceAccount.Id);
        Assert.AreEqual(sourceAccount.Id, _viewModel.Model.SourceAccountId);
    }

    [TestMethod]
    public void UpdatingDestinationAccount_UpdatesModel()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        var destinationAccount = new AccountListItemDto
        {
            Id = Guid.NewGuid(),
            AccountTypeId = AccountConstants.Asset,
            Name = "Test asset account"
        };
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState(
            destinationAccount));
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetDropdownValueByLabel("Destination account", destinationAccount.Id);
        Assert.AreEqual(destinationAccount.Id, _viewModel.Model.DestinationAccountId);
    }

    [TestMethod]
    public void UpdatingDate_UpdatesModel()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        DateOnly date = new DateOnly(2022, 8, 15);
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetDateValueByLabel("Date", date);
        Assert.AreEqual(date, _viewModel.Model.Date);
    }

    [TestMethod]
    public void UpdatingAmount_UpdatesModel()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        decimal amount = 123.42M;
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetCurrencyValueByLabel("Amount", amount);
        Assert.AreEqual(amount, _viewModel.Model.Amount);
    }

    [TestMethod]
    public void UpdatingForeignCurrencyId_UpdatesModel()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetDropdownValueByLabel("Foreign currency", Currency.NZD);
        Assert.AreEqual(Currency.NZD, _viewModel.Model.ForeignCurrencyId);
    }

    [TestMethod]
    public void UpdatingForeignAmount_UpdatesModel()
    {
        Guid transactionTypeId = TransactionConstants.Deposit;
        decimal amount = 123.42M;
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());

        var cut = RenderComponent<AddTransactionForm>(_ => _
            .Add(_ => _.TransactionType, transactionTypeId)
            .Add(_ => _.LookupState, _stateStore.GetState<LookupState>().Instance)
            .Add(_ => _.AccountListState, _stateStore.GetState<AccountListState>().Instance)
            .Add(_ => _.ViewModel, _viewModel));

        cut.SetCurrencyValueByLabel("Foreign amount", amount);
        Assert.AreEqual(amount, _viewModel.Model.ForeignAmount);
    }
}
