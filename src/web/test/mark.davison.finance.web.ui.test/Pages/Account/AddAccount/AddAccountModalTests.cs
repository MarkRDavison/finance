using mark.davison.finance.web.features.Account.Add;
using mark.davison.finance.web.features.Account.Create;

namespace mark.davison.finance.web.ui.test.Pages.Account.AddAccount;

[TestClass]
public class AddAccountModalTests : TestBase
{
    private readonly AddAccountViewModel _viewModel;
    private readonly Mock<IStateHelper> _stateHelper;

    public AddAccountModalTests()
    {
        _viewModel = new(_dispatcher.Object);
        _stateHelper = new(MockBehavior.Strict);
    }

    protected override void SetupTest()
    {
        base.SetupTest();
        Services.Add(new ServiceDescriptor(typeof(AddAccountViewModel), _viewModel));
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

        _viewModel.AddAccountFormViewModel = new()
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

        var cut = RenderComponent<AddAccountModal>(_ => _
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
