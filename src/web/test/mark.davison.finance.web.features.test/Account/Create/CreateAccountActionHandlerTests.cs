namespace mark.davison.finance.web.features.test.Account.Create;

[TestClass]
public class CreateAccountActionHandlerTests
{
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly CreateAccountActionHandler _handler;

    public CreateAccountActionHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _handler = new(_repository.Object);
    }


    [TestMethod]
    public async Task Handle_InvokesRepostory()
    {
        var accountListItems = new List<AccountListItemDto> {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        _repository
            .Setup(_ => _
                .Post<UpsertAccountResponse, UpsertAccountRequest>(
                    It.IsAny<UpsertAccountRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpsertAccountRequest req, CancellationToken cancellationToken) => new UpsertAccountResponse()
            {
                Success = true
            })
            .Verifiable();

        var response = await _handler.Handle(new CreateAccountCommandRequest(), CancellationToken.None);

        Assert.AreNotEqual(Guid.Empty, response.ItemId);

        _repository
            .Verify(_ => _
                .Post<UpsertAccountResponse, UpsertAccountRequest>(
                    It.IsAny<UpsertAccountRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
