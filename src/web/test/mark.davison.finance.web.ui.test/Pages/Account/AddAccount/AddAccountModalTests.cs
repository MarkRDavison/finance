namespace mark.davison.finance.web.ui.test.Pages.Account.EditAccount;

[TestClass]
public class EditAccountModalTests : TestBase
{
    private readonly EditAccountViewModel _viewModel;
    private readonly Mock<IStateHelper> _stateHelper;

    public EditAccountModalTests()
    {
        _viewModel = new(_dispatcher.Object);
        _stateHelper = new(MockBehavior.Strict);
    }

    protected override void SetupTest()
    {
        base.SetupTest();
        Services.Add(new ServiceDescriptor(typeof(EditAccountViewModel), _viewModel));
        Services.Add(new ServiceDescriptor(typeof(IStateHelper), _stateHelper.Object));

        _stateHelper.Setup(_ => _.FetchAccountList(It.IsAny<bool>())).Returns(Task.CompletedTask);
        _stateHelper.Setup(_ => _.FetchCategoryList()).Returns(Task.CompletedTask);
    }

    [TestMethod]
    public async Task SubmittingFormInvokesDispatcher()
    {
        _stateStore.SetState(AccountListStateHelpers.CreateAccountListState());
        _stateStore.SetState(LookupStateHelpers.CreateStandardState());
        _stateStore.SetState(CategoryListStateHelpers.CreateCategoryListState());

        _viewModel.EditAccountFormViewModel = new()
        {
            Name = "name",
            AccountNumber = "acc num",
            AccountTypeId = AccountConstants.Asset,
            CurrencyId = Currency.NZD
        };

        _dispatcher
            .Setup(_ => _.Dispatch<CreateAccountCommandRequest, CreateAccountCommandResponse>(
                It.IsAny<CreateAccountCommandRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateAccountCommandResponse() { Success = true, ItemId = Guid.NewGuid() })
            .Verifiable();

        _dispatcher
            .Setup(_ => _.Dispatch<UpdateAccountListItemsAction>(
                It.IsAny<UpdateAccountListItemsAction>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var cut = RenderComponent<EditAccountModal>(_ => _
            .Add(_ => _.IsOpen, true));

        await cut
            .FindAll("button")
            .First(_ => _.TextContent == "Submit")
            .ClickAsync(new MouseEventArgs());

        _dispatcher
            .Verify(
                _ => _.Dispatch<UpdateAccountListItemsAction>(
                    It.IsAny<UpdateAccountListItemsAction>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _dispatcher
            .Verify(
                _ => _.Dispatch<CreateAccountCommandRequest, CreateAccountCommandResponse>(
                    It.IsAny<CreateAccountCommandRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
