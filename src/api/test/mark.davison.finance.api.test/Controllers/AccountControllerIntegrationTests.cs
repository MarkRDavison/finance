namespace mark.davison.finance.api.test.Controllers;

[TestClass]
public class AccountControllerIntegrationTests : IntegrationTestBase<FinanceApiWebApplicationFactory, AppSettings>
{
    private readonly List<Account> _accounts = new();
    private readonly List<Transaction> _transactions = new();
    private readonly List<TransactionJournal> _transactionJournals = new();
    private readonly List<TransactionGroup> _transactionGroups = new();

    [TestMethod]
    public async Task GetAccountSummaries_ReturnsAccountInfo()
    {
        var accountSummaries = await GetMultipleAsync<AccountSummary>("/api/account/summary");
        Assert.AreEqual(_accounts.Count, accountSummaries.Count);
        _accounts.ForEach(_ =>
        {
            var match = accountSummaries.FirstOrDefault(__ => __.Id == _.Id);
            Assert.IsNotNull(match);
            Assert.AreEqual(_.Name, match.Name);
            Assert.AreEqual(_.AccountNumber, match.AccountNumber);
        });
    }

    [TestMethod]
    public async Task GetAccountSummaries_ReturnsOpeningBalanceInfo()
    {
        var accountSummaries = await GetMultipleAsync<AccountSummary>("/api/account/summary");
        var accountSummaryWithOpeningBalance = accountSummaries.First(_ => _.AccountNumber == "3");

        Assert.IsNotNull(accountSummaryWithOpeningBalance);

        Assert.AreNotEqual(0, accountSummaryWithOpeningBalance.OpeningBalance);
    }

    protected override async Task SeedData(IServiceProvider serviceProvider)
    {
        var repository = serviceProvider.GetRequiredService<IRepository>();
        _accounts.AddRange(new List<Account> {
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "Asset Account",
                AccountTypeId = AccountConstants.Asset,
                CurrencyId = Currency.NZD,
                AccountNumber = "1",
                IsActive = true
            },
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "Revenue Account",
                AccountTypeId = AccountConstants.Revenue,
                CurrencyId = Currency.NZD,
                AccountNumber = "2",
                IsActive = false
            },
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "Asset Account with opening balance",
                AccountTypeId = AccountConstants.Revenue,
                CurrencyId = Currency.NZD,
                AccountNumber = "3",
                IsActive = true
            }
        });

        _transactionGroups.Add(new TransactionGroup
        {
            Id = Guid.NewGuid()
        });
        _transactionJournals.AddRange(new List<TransactionJournal>
        {
            new TransactionJournal
            {
                Id = Guid.NewGuid(),
                TransactionGroupId = _transactionGroups[0].Id,
                TransactionTypeId = TransactionConstants.OpeningBalance,
                CurrencyId = Currency.NZD
            }
        });
        _transactions.AddRange(new List<Transaction>
        {
            new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionJournalId = _transactionJournals[0].Id,
                AccountId = Account.OpeningBalance,
                CurrencyId = Currency.NZD,
                Amount = -CurrencyRules.ToPersisted(100.0M)
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionJournalId = _transactionJournals[0].Id,
                AccountId = _accounts.First(_ => _.AccountNumber == "3").Id,
                CurrencyId = Currency.NZD,
                Amount = +CurrencyRules.ToPersisted(100.0M)
            }
        });

        await using (repository.BeginTransaction())
        {
            await repository.UpsertEntitiesAsync(_accounts, CancellationToken.None);
            await repository.UpsertEntitiesAsync(_transactionGroups, CancellationToken.None);
            await repository.UpsertEntitiesAsync(_transactionJournals, CancellationToken.None);
            await repository.UpsertEntitiesAsync(_transactions, CancellationToken.None);
        }
    }

}

