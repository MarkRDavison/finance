﻿namespace mark.davison.finance.web.features.test.Account.List;


[TestClass]
public class FetchCategoryListActionHandlerTests
{
    private readonly Mock<IStateStore> _stateStore;
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly FetchAccountListActionHandler _handler;

    public FetchCategoryListActionHandlerTests()
    {
        _stateStore = new(MockBehavior.Strict);
        _repository = new(MockBehavior.Strict);
        _handler = new(_repository.Object, _stateStore.Object);
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

        _repository
            .Setup(_ => _
                .Get<AccountListQueryResponse, AccountListQueryRequest>(
                    It.IsAny<AccountListQueryRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountListQueryRequest req, CancellationToken cancellationToken) => new AccountListQueryResponse()
            {
                Accounts = accountListItems
            })
            .Verifiable();

        await _handler.Handle(new FetchAccountListAction(true), CancellationToken.None);

        _repository
            .Verify(_ => _
                .Get<AccountListQueryResponse, AccountListQueryRequest>(
                    It.IsAny<AccountListQueryRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

        _stateStore
            .Verify(_ => _
                .SetState<AccountListState>(
                    It.IsAny<AccountListState>()),
                Times.Once);
    }
}