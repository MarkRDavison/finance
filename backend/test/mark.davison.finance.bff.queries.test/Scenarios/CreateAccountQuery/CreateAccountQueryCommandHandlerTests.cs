namespace mark.davison.finance.bff.queries.test.Scenarios.CreateAccountQuery;

[TestClass]
public class CreateAccountQueryCommandHandlerTests
{

    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly CreateAccountQueryCommandHandler _createAccountQueryCommandHandler;

    public CreateAccountQueryCommandHandlerTests()
    {
        _httpRepositoryMock = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _createAccountQueryCommandHandler = new CreateAccountQueryCommandHandler(_httpRepositoryMock.Object);
    }

    [TestMethod]
    public async Task SearchItemsForBanksAndAccountTypesAreReturnedCorrectly()
    {
        var banks = new List<Bank> {
            new Bank { Id = Guid.NewGuid(), Name = "Kiwibank" },
            new Bank { Id = Guid.NewGuid(), Name = "ANZ" },
            new Bank { Id = Guid.NewGuid(), Name = "BNZ" },
            new Bank { Id = Guid.NewGuid(), Name = "Westpac" }
        };
        var accountTypes = new List<AccountType> {
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountType.Default) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountType.Debt) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountType.Asset) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountType.Beneficiary) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountType.Cash) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountType.Expense) }
        };

        _httpRepositoryMock.Setup(_ => _.GetEntitiesAsync<Bank>(
            It.IsAny<QueryParameters>(),
            It.IsAny<HeaderParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(banks);
        _httpRepositoryMock.Setup(_ => _.GetEntitiesAsync<AccountType>(
            It.IsAny<QueryParameters>(),
            It.IsAny<HeaderParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountTypes);

        var request = new CreateAccountQueryRequest { };

        var response = await _createAccountQueryCommandHandler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(banks.Count, response.Banks.Count);
        Assert.AreEqual(accountTypes.Count, response.AccountTypes.Count);
        foreach (var b in banks)
        {
            Assert.IsTrue(response.Banks.Exists(_ =>
                _.Id == b.Id &&
                _.PrimaryText == b.Name));
        }
        foreach (var at in accountTypes)
        {
            Assert.IsTrue(response.AccountTypes.Exists(_ =>
                _.Id == at.Id &&
                _.PrimaryText == at.Type));
        }
    }

}

