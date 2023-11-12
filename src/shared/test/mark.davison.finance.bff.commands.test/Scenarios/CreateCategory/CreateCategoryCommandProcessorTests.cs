namespace mark.davison.finance.bff.commands.test.Scenarios.CreateCategory;

[TestClass]
public class CreateCategoryCommandProcessorTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;

    private readonly CreateCategoryCommandProcessor _processor;

    public CreateCategoryCommandProcessorTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { }); // TODO: Extract out mocking of current user context

        _repository.Setup(_ => _.BeginTransaction()).Returns(() => new TestAsyncDisposable());

        _processor = new(_repository.Object);
    }

    [TestMethod]
    public async Task ProcessAsync_WhereCategoryCreated_ReturnsSuccess()
    {
        var request = new CreateCategoryCommandRequest
        {
            Id = Guid.NewGuid(),
            Name = "NEW CATEGORY"
        };

        _repository
            .Setup(_ => _.UpsertEntityAsync(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken ct) =>
            {
                Assert.AreEqual(request.Id, c.Id);
                Assert.AreEqual(request.Name, c.Name);

                return c;
            })
            .Verifiable();

        var response = await _processor.ProcessAsync(request, _currentUserContext.Object, CancellationToken.None);

        Assert.IsTrue(response.Success);

        _repository
            .Verify(
                _ => _.UpsertEntityAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
