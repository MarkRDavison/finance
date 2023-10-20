namespace mark.davison.finance.bff.commands.test.integration.Scenarios.UpsertAccount;

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
                AccountTypeId = AccountConstants.Asset
            }
        };
        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var repository = GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            var account = await repository.GetEntityAsync<Account>(request.UpsertAccountDto.Id);
            Assert.IsNotNull(account);

            var transactionJournal = await repository.GetEntityAsync<TransactionJournal>(_ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id));
            Assert.IsNull(transactionJournal);
        }
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
                AccountTypeId = AccountConstants.Asset,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var repository = GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            var account = await repository.GetEntityAsync<Account>(request.UpsertAccountDto.Id);
            Assert.IsNotNull(account);

            var includes = new Expression<Func<TransactionJournal, object>>[] {
                _ => _.TransactionGroup!
            };
            var transactionJournal = await repository.GetEntityAsync<TransactionJournal>(
                _ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id),
                includes,
                CancellationToken.None);
            Assert.IsNotNull(transactionJournal);
        }
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
                AccountTypeId = AccountConstants.Asset
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

        var repository = GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            var includes = new Expression<Func<TransactionJournal, object>>[] {
                _ => _.TransactionGroup!
            };
            var transactionJournal = await repository.GetEntityAsync<TransactionJournal>(
                _ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id),
                includes,
                CancellationToken.None);

            Assert.IsNotNull(transactionJournal);
        }
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
                AccountTypeId = AccountConstants.Asset,
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

        var repository = GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            var account = await repository.GetEntityAsync<Account>(request.UpsertAccountDto.Id);
            Assert.IsNotNull(account);

            var transactionJournal = await repository.GetEntityAsync<TransactionJournal>(
                _ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id),
                includes,
                CancellationToken.None);
            Assert.IsNotNull(transactionJournal);
        }

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

        repository = GetRequiredService<IRepository>();
        await using (repository.BeginTransaction())
        {
            var transactionJournal = await repository.GetEntityAsync<TransactionJournal>(
            _ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id),
            includes,
            CancellationToken.None);

            Assert.IsNull(transactionJournal);
        }
    }

}
