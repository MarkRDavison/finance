namespace mark.davison.finance.shared.commands.test.integration.Scenarios.UpsertAccount;

[TestClass]
public class UpsertAccountTests : CQRSIntegrationTestBase
{

    [TestMethod]
    public async Task AddingAccountWithoutOpeningBalance_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = "1234",
                Name = "No opening balance",
                CurrencyId = Currency.NZD,
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeConstants.Asset
            }
        };
        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var dbContext = GetRequiredService<IFinanceDbContext>();

        var account = await dbContext.GetByIdAsync<Account>(request.UpsertAccountDto.Id, CancellationToken.None);
        Assert.IsNotNull(account); // TODO: AnyAsync

        var transactionJournal = await dbContext
            .Set<TransactionJournal>()
            .Where(_ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id))
            .FirstOrDefaultAsync(CancellationToken.None);
        Assert.IsNull(transactionJournal);
    }

    [TestMethod]
    public async Task AddingAccountWithOpeningBalance_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = "1234",
                Name = "Opening balance",
                CurrencyId = Currency.NZD,
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeConstants.Asset,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var dbContext = GetRequiredService<IFinanceDbContext>();

        var account = await dbContext.GetByIdAsync<Account>(request.UpsertAccountDto.Id, CancellationToken.None);
        Assert.IsNotNull(account); // TODO: AnyAsync

        var transactionJournal = await dbContext
            .Set<TransactionJournal>()
            .Include(_ => _.TransactionGroup)
            .Where(_ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id))
            .FirstOrDefaultAsync(CancellationToken.None);
        Assert.IsNotNull(transactionJournal);
    }

    [TestMethod]
    public async Task AddingAccountWithoutOpeningBalance_ThenUpsertingNewOpeningBalance_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = "1234",
                Name = "No initial opening balance",
                CurrencyId = Currency.NZD,
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeConstants.Asset
            }
        };

        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = request.UpsertAccountDto.AccountNumber,
                Name = request.UpsertAccountDto.Name,
                CurrencyId = request.UpsertAccountDto.CurrencyId,
                Id = request.UpsertAccountDto.Id,
                AccountTypeId = request.UpsertAccountDto.AccountTypeId,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var dbContext = GetRequiredService<IFinanceDbContext>();

        var account = await dbContext.GetByIdAsync<Account>(request.UpsertAccountDto.Id, CancellationToken.None);
        Assert.IsNotNull(account); // TODO: AnyAsync

        var transactionJournal = await dbContext
            .Set<TransactionJournal>()
            .Include(_ => _.TransactionGroup)
            .Where(_ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id))
            .FirstOrDefaultAsync(CancellationToken.None);
        Assert.IsNotNull(transactionJournal);
    }

    [TestMethod]
    public async Task AddingAccountWithOpeningBalance_ThenRemovingIt_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = "1234",
                Name = "Opening balance",
                CurrencyId = Currency.NZD,
                Id = Guid.NewGuid(),
                AccountTypeId = AccountTypeConstants.Asset,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var includes = new Expression<Func<TransactionJournal, object>>[]
        {
            _ => _.TransactionGroup!
        };

        var dbContext = GetRequiredService<IFinanceDbContext>();

        var account = await dbContext.GetByIdAsync<Account>(request.UpsertAccountDto.Id, CancellationToken.None);
        Assert.IsNotNull(account); // TODO: AnyAsync

        var transactionJournal = await dbContext
            .Set<TransactionJournal>()
            .Include(_ => _.TransactionGroup)
            .Where(_ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id))
            .FirstOrDefaultAsync(CancellationToken.None);
        Assert.IsNotNull(transactionJournal);

        request = new UpsertAccountCommandRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = request.UpsertAccountDto.AccountNumber,
                Name = request.UpsertAccountDto.Name,
                CurrencyId = request.UpsertAccountDto.CurrencyId,
                Id = request.UpsertAccountDto.Id,
                AccountTypeId = request.UpsertAccountDto.AccountTypeId
            }
        };

        response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        account = await dbContext.GetByIdAsync<Account>(request.UpsertAccountDto.Id, CancellationToken.None);
        Assert.IsNotNull(account); // TODO: AnyAsync

        transactionJournal = await dbContext
            .Set<TransactionJournal>()
            .Include(_ => _.TransactionGroup)
            .Where(_ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id))
            .FirstOrDefaultAsync(CancellationToken.None);
        Assert.IsNull(transactionJournal);
    }

}
