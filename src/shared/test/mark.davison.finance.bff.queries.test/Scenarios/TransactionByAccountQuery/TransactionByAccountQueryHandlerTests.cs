namespace mark.davison.finance.bff.queries.test.Scenarios.TransactionByAccountQuery;

[TestClass]
public class TransactionByAccountQueryHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly TransactionByAccountQueryHandler _handler;

    public TransactionByAccountQueryHandlerTests()
    {
        _httpRepositoryMock = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new TransactionByAccountQueryHandler(_httpRepositoryMock.Object);
    }

    [TestMethod]
    public void TODO()
    {
        Assert.Fail();
    }
}
