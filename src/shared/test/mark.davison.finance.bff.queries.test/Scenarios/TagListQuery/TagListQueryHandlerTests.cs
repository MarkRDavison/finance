namespace mark.davison.finance.bff.queries.test.Scenarios.TagListQuery;

[TestClass]
public class TagListQueryHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TagListQueryHandler _handler;

    public TagListQueryHandlerTests()
    {
        _httpRepository = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new TagListQueryHandler(_httpRepository.Object);
    }

    [TestMethod]
    public async Task Handle_RetrievesTagsFromRepository()
    {
        var tags = new List<Tag> {
            new Tag{ },
            new Tag{ }
        };

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Tag>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                Assert.IsTrue(q.ContainsKey(nameof(Tag.UserId)));
                Assert.AreEqual(
                    _currentUserContext.Object.CurrentUser.Id.ToString(),
                    q[nameof(Tag.UserId)]);
                return tags;
            })
            .Verifiable();

        var request = new TagListQueryRequest();
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(tags.Count, response.Tags.Count);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<Tag>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
