using mark.davison.finance.web.ui.Features.Account.Add;

namespace mark.davison.finance.web.ui.test.Features.Account.Add;

[TestClass]
public class UpdateAccountListItemsActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly UpdateAccountListItemsActionHandler _handler;

    public UpdateAccountListItemsActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _handler = new(_stateStore.Object);
    }


    [TestMethod]
    public async Task Handle_InvokesRepostoryAndStateStore()
    {
        var accountListItems = new List<AccountListItemDto> {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        _stateStore
            .Setup(_ => _
                .GetState<AccountListState>())
            .Returns(new StateInstance<AccountListState>(() => new AccountListState()));
        _stateStore
            .Setup(_ => _
                .SetState<AccountListState>(
                    It.IsAny<AccountListState>()))
            .Callback((AccountListState newState) =>
            {
                Assert.AreEqual(accountListItems.Count, newState.Accounts.Count());
            })
            .Verifiable();

        await _handler.Handle(new UpdateAccountListItemsAction(accountListItems), CancellationToken.None);


        _stateStore
            .Verify(_ => _
                .SetState<AccountListState>(
                    It.IsAny<AccountListState>()),
                Times.Once);
    }
}
