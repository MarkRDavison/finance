using mark.davison.finance.common.server.abstractions.Authentication;
using mark.davison.finance.common.server.abstractions.Repository;

namespace mark.davison.finance.bff.queries.test.Scenarios.AccountListQuery;

public class AccountListQueryCommandHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly AccountListQueryCommandHandler _handler;

    public AccountListQueryCommandHandlerTests()
    {
        _httpRepositoryMock = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new AccountListQueryCommandHandler(_httpRepositoryMock.Object);
    }
}

