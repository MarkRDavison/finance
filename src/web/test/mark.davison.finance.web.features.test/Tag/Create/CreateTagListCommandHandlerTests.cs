namespace mark.davison.finance.web.features.test.Tag.Create;

[TestClass]
public class CreateTagListCommandHandlerTests
{
    private readonly Mock<IClientHttpRepository> _repository;
    private readonly CreateTagListCommandHandler _handler;

    public CreateTagListCommandHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _handler = new(_repository.Object);
    }


    [TestMethod]
    public async Task Handle_InvokesRepostory()
    {
        var items = new List<TagDto> {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() }
        };

        _repository
            .Setup(_ => _
                .Post<CreateTagCommandResponse, CreateTagCommandRequest>(
                    It.IsAny<CreateTagCommandRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTagCommandRequest req, CancellationToken cancellationToken) => new CreateTagCommandResponse()
            {
                Success = true
            })
            .Verifiable();

        var response = await _handler.Handle(new CreateTagListCommandRequest(), CancellationToken.None);

        Assert.AreNotEqual(Guid.Empty, response.ItemId);

        _repository
            .Verify(_ => _
                .Post<CreateTagCommandResponse, CreateTagCommandRequest>(
                    It.IsAny<CreateTagCommandRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
