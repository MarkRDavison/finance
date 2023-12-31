﻿namespace mark.davison.finance.bff.queries.test.Scenarios.StartupQuery;

[TestClass]
public class StartupQueryCommandHandlerTests
{
    private readonly Mock<IRepository> _repository;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly StartupQueryCommandHandler _handler;

    public StartupQueryCommandHandlerTests()
    {
        _repository = new(MockBehavior.Strict);
        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new StartupQueryCommandHandler(_repository.Object);

        _repository.Setup(_ => _.BeginTransaction()).Returns(() => new TestAsyncDisposable());
    }

    [TestMethod]
    public async Task DtosForAccountTypes_Currencies_ReturnedCorrectly()
    {
        var accountTypes = new List<AccountType> {
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountConstants.Default) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountConstants.Debt) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountConstants.Asset) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountConstants.Beneficiary) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountConstants.Cash) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountConstants.Expense) }
        };
        var currencies = new List<Currency> {
            new Currency { Id = Guid.NewGuid(), Code = "NZD", Name = "NZ Dollar", Symbol = "NZ$", DecimalPlaces = 2 },
            new Currency { Id = Currency.INT, Code = "INT", Name = "NZ Dollar", Symbol = "NZ$", DecimalPlaces = 2 }
        };
        var transactionTypes = new List<TransactionType> {
            new TransactionType { Id = Guid.NewGuid(), Type = nameof(TransactionConstants.OpeningBalance) },
            new TransactionType { Id = Guid.NewGuid(), Type = nameof(TransactionConstants.Invalid) },
            new TransactionType { Id = Guid.NewGuid(), Type = nameof(TransactionConstants.Deposit) },
        };

        _repository.Setup(_ => _.GetEntitiesAsync<AccountType>(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountTypes);
        _repository.Setup(_ => _.GetEntitiesAsync<Currency>(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);
        _repository.Setup(_ => _.GetEntitiesAsync<TransactionType>(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionTypes);

        var request = new StartupQueryRequest { };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        Assert.AreEqual(accountTypes.Count, response.AccountTypes.Count);
        Assert.AreEqual(currencies.Count(_ => _.Id != Currency.INT), response.Currencies.Count);
        Assert.AreEqual(transactionTypes.Count, response.TransactionTypes.Count);

        foreach (var at in accountTypes)
        {
            Assert.IsTrue(response.AccountTypes.Exists(_ =>
                _.Id == at.Id &&
                _.Type == at.Type));
        }
        foreach (var c in currencies)
        {
            if (c.Id == Currency.INT)
            {
                Assert.IsFalse(response.Currencies.Exists(_ =>
                    _.Id == c.Id));
            }
            else
            {
                Assert.IsTrue(response.Currencies.Exists(_ =>
                    _.Id == c.Id &&
                    _.Code == c.Code &&
                    _.Name == c.Name &&
                    _.Symbol == c.Symbol &&
                    _.DecimalPlaces == c.DecimalPlaces));
            }
        }
        foreach (var at in transactionTypes)
        {
            Assert.IsTrue(response.TransactionTypes.Exists(_ =>
                _.Id == at.Id &&
                _.Type == at.Type));
        }
    }
}

