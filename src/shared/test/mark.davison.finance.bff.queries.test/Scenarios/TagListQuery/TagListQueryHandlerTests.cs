using System.Linq.Expressions;

namespace mark.davison.finance.bff.queries.test.Scenarios.TagListQuery;

[TestClass]
public class TagListQueryHandlerTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TagListQueryHandler _handler;

    public TagListQueryHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new TagListQueryHandler(_repository.Object);
    }

    [TestMethod]
    public async Task Handle_RetrievesTagsFromRepository()
    {
        var tags = new List<Tag> {
            new Tag{ },
            new Tag{ }
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<Tag>(
                It.IsAny<Expression<Func<Tag, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags)
            .Verifiable();

        var request = new TagListQueryRequest();
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(tags.Count, response.Tags.Count);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Tag>(
                    It.IsAny<Expression<Func<Tag, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
