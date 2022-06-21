using mark.davison.finance.bff.queries.Scenarios.StartupQuery;

namespace mark.davison.finance.bff.queries.test.Scenarios.StartupQuery;

[TestClass]
public class StartupQueryCommandHandlerTests
{
    private readonly Mock<IHttpRepository> _httpRepositoryMock;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly StartupQueryCommandHandler _handler;

    public StartupQueryCommandHandlerTests()
    {
        _httpRepositoryMock = new Mock<IHttpRepository>(MockBehavior.Strict);
        _currentUserContext = new Mock<ICurrentUserContext>(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new StartupQueryCommandHandler(_httpRepositoryMock.Object);
    }

    [TestMethod]
    public async Task DtosForAccountTypes_Banks_Currencies_ReturnedCorrectly()
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
        var currencies = new List<Currency> {
            new Currency { Id = Guid.NewGuid(), Code = "NZD", Name = "NZ Dollar", Symbol = "NZ$", DecimalPlaces = 2 }
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
        _httpRepositoryMock.Setup(_ => _.GetEntitiesAsync<Currency>(
            It.IsAny<QueryParameters>(),
            It.IsAny<HeaderParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);

        var request = new StartupQueryRequest { };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(banks.Count, response.Banks.Count);
        Assert.AreEqual(accountTypes.Count, response.AccountTypes.Count);
        Assert.AreEqual(currencies.Count, response.Currencies.Count);
        foreach (var b in banks)
        {
            Assert.IsTrue(response.Banks.Exists(_ =>
                _.Id == b.Id &&
                _.Name == b.Name));
        }
        foreach (var at in accountTypes)
        {
            Assert.IsTrue(response.AccountTypes.Exists(_ =>
                _.Id == at.Id &&
                _.Type == at.Type));
        }
        foreach (var c in currencies)
        {
            Assert.IsTrue(response.Currencies.Exists(_ =>
                _.Id == c.Id &&
                _.Code == c.Code &&
                _.Name == c.Name &&
                _.Symbol == c.Symbol &&
                _.DecimalPlaces == c.DecimalPlaces));
        }
    }
}

