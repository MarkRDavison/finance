using System.Linq.Expressions;

namespace mark.davison.finance.bff.queries.test.Scenarios.CategoryListQuery;

[TestClass]
public class CategoryListQueryHandlerTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CategoryListQueryHandler _handler;

    public CategoryListQueryHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new CategoryListQueryHandler(_repository.Object);
    }

    [TestMethod]
    public async Task Handle_RetrievesCategoriesFromRepository()
    {
        var categories = new List<Category> {
            new Category{ },
            new Category{ }
        };

        _repository
            .Setup(_ => _.GetEntitiesAsync<Category>(
                It.IsAny<Expression<Func<Category, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories)
            .Verifiable();

        var request = new CategoryListQueryRequest();
        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(categories.Count, response.Categories.Count);

        _repository
            .Verify(
                _ => _.GetEntitiesAsync<Category>(
                    It.IsAny<Expression<Func<Category, bool>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}

