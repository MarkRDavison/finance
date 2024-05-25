﻿namespace mark.davison.finance.shared.queries.test.Scenarios.StartupQuery;

[TestClass]
public class StartupQueryCommandHandlerTests
{
    private readonly IDbContext<FinanceDbContext> _dbContext;
    private readonly Mock<ICurrentUserContext> _currentUserContext;
    private readonly StartupQueryHandler _handler;
    private readonly CancellationToken _token;

    public StartupQueryCommandHandlerTests()
    {
        _token = CancellationToken.None;
        _dbContext = DbContextHelpers.CreateInMemory<FinanceDbContext>(_ => new FinanceDbContext(_));

        _currentUserContext = new(MockBehavior.Strict);
        _currentUserContext.Setup(_ => _.Token).Returns("");
        _currentUserContext.Setup(_ => _.CurrentUser).Returns(new User { });

        _handler = new StartupQueryHandler((IFinanceDbContext)_dbContext);
    }

    [TestMethod]
    public async Task DtosForAccountTypes_Currencies_ReturnedCorrectly()
    {
        var accountTypes = new List<AccountType> {
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountTypeConstants.Default) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountTypeConstants.Debt) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountTypeConstants.Asset) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountTypeConstants.Beneficiary) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountTypeConstants.Cash) },
            new AccountType { Id = Guid.NewGuid(), Type = nameof(AccountTypeConstants.Expense) }
        };
        var currencies = new List<Currency> {
            new Currency { Id = Guid.NewGuid(), Code = "NZD", Name = "NZ Dollar", Symbol = "NZ$", DecimalPlaces = 2 },
            new Currency { Id = Currency.INT, Code = "INT", Name = "NZ Dollar", Symbol = "NZ$", DecimalPlaces = 2 }
        };
        var transactionTypes = new List<TransactionType> {
            new TransactionType { Id = Guid.NewGuid(), Type = nameof(TransactionTypeConstants.OpeningBalance) },
            new TransactionType { Id = Guid.NewGuid(), Type = nameof(TransactionTypeConstants.Invalid) },
            new TransactionType { Id = Guid.NewGuid(), Type = nameof(TransactionTypeConstants.Deposit) },
        };

        await _dbContext.UpsertEntitiesAsync(accountTypes, _token);
        await _dbContext.UpsertEntitiesAsync(currencies, _token);
        await _dbContext.UpsertEntitiesAsync(transactionTypes, _token);
        await _dbContext.SaveChangesAsync(_token);

        var request = new StartupQueryRequest { };

        var response = await _handler.Handle(request, _currentUserContext.Object, CancellationToken.None);

        response.Success.Should().BeTrue();
        response.Value.Should().NotBeNull();

        response.Value!.AccountTypes.Should().HaveCount(accountTypes.Count);
        response.Value.Currencies.Should().HaveCount(currencies.Count(_ => _.Id != Currency.INT));
        response.Value.TransactionTypes.Should().HaveCount(transactionTypes.Count);

        foreach (var at in accountTypes)
        {
            response.Value.AccountTypes
                .Where(_ =>
                    _.Id == at.Id &&
                    _.Type == at.Type)
                .Should()
                .HaveCount(1);
        }
        foreach (var c in currencies)
        {
            if (c.Id == Currency.INT)
            {
                response.Value.Currencies
                    .Where(_ => _.Id == c.Id)
                    .Should()
                    .HaveCount(0);
            }
            else
            {
                response.Value.Currencies
                    .Where(_ =>
                        _.Id == c.Id &&
                        _.Code == c.Code &&
                        _.Name == c.Name &&
                        _.Symbol == c.Symbol &&
                        _.DecimalPlaces == c.DecimalPlaces)
                    .Should()
                    .HaveCount(1);
            }
        }
        foreach (var tt in transactionTypes)
        {
            response.Value.TransactionTypes
                .Where(_ =>
                    _.Id == tt.Id &&
                    _.Type == tt.Type)
                .Should()
                .HaveCount(1);
        }
    }
}
