namespace mark.davison.finance.bff.queries.test.Scenarios.CategoryListQuery;

[TestClass]
public class CategoryListQueryHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CategoryListQueryHandler _handler;

    public CategoryListQueryHandlerTests()
    {
        _httpRepository = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new CategoryListQueryHandler(_httpRepository.Object);
    }

    [TestMethod]
    public async Task Handle_RetrievesCategoriesFromRepository()
    {
        var categories = new List<Category> {
            new Category{ },
            new Category{ }
        };

        _httpRepository
            .Setup(_ => _.GetEntitiesAsync<Category>(
                It.IsAny<QueryParameters>(),
                It.IsAny<HeaderParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueryParameters q, HeaderParameters h, CancellationToken c) =>
            {
                Assert.IsTrue(q.ContainsKey(nameof(Category.UserId)));
                Assert.AreEqual(
                    _currentUserContext.Object.CurrentUser.Id.ToString(),
                    q[nameof(Category.UserId)]);
                return categories;
            })
            .Verifiable();

        var request = new CategoryListQueryRequest();
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(categories.Count, response.Categories.Count);

        _httpRepository
            .Verify(
                _ => _.GetEntitiesAsync<Category>(
                    It.IsAny<QueryParameters>(),
                    It.IsAny<HeaderParameters>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

