using mark.davison.finance.accounting.rules;
using mark.davison.finance.models.dtos.Commands.UpsertAccount;
using System.Linq.Expressions;

namespace mark.davison.finance.bff.commands.test.integration.Scenarios.UpsertAccount;

[TestClass]
public class UpsertAccountTests : CommandIntegrationTestBase
{

    [TestMethod]
    public async Task AddingAccountWithoutOpeningBalance_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountRequest, UpsertAccountResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountRequest
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
        var account = await repository.GetEntityAsync<Account>(request.UpsertAccountDto.Id);
        Assert.IsNotNull(account);

        var transactionJournal = await repository.GetEntityAsync<TransactionJournal>(_ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id));
        Assert.IsNull(transactionJournal);
    }

    [TestMethod]
    public async Task AddingAccountWithOpeningBalance_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountRequest, UpsertAccountResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountRequest
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

    [TestMethod]
    public async Task AddingAccountWithoutOpeningBalance_ThenUpsertingNewOpeningBalance_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountRequest, UpsertAccountResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountRequest
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

        var repository = GetRequiredService<IRepository>();
        var account = await repository.GetEntityAsync<Account>(request.UpsertAccountDto.Id);
        Assert.IsNotNull(account);

        request = new UpsertAccountRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = account.AccountNumber,
                Name = account.Name,
                CurrencyId = account.CurrencyId,
                Id = account.Id,
                AccountTypeId = account.AccountTypeId,
                OpeningBalance = CurrencyRules.ToPersisted(100.0M),
                OpeningBalanceDate = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        var includes = new Expression<Func<TransactionJournal, object>>[] {
            _ => _.TransactionGroup!
        };
        var transactionJournal = await repository.GetEntityAsync<TransactionJournal>(
            _ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id),
            includes,
            CancellationToken.None);

        Assert.IsNotNull(transactionJournal);
    }

    [TestMethod]
    public async Task AddingAccountWithOpeningBalance_ThenRemovingIt_Works()
    {
        var handler = GetRequiredService<ICommandHandler<UpsertAccountRequest, UpsertAccountResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new UpsertAccountRequest
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

        request = new UpsertAccountRequest
        {
            UpsertAccountDto = new UpsertAccountDto
            {
                AccountNumber = account.AccountNumber,
                Name = account.Name,
                CurrencyId = account.CurrencyId,
                Id = account.Id,
                AccountTypeId = account.AccountTypeId
            }
        };

        response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);

        transactionJournal = await repository.GetEntityAsync<TransactionJournal>(
            _ => _.Transactions.Any(_ => _.AccountId == request.UpsertAccountDto.Id),
            includes,
            CancellationToken.None);

        Assert.IsNull(transactionJournal);
    }

}
