namespace mark.davison.finance.api.test.Controllers;

[TestClass]
public class AccountControllerIntegrationTests : IntegrationTestBase<FinanceApiWebApplicationFactory, AppSettings>
{
    private readonly List<Account> _accounts = new List<Account>();

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

    protected override async Task SeedData(IRepository repository)
    {
        _accounts.AddRange(new List<Account> {
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "Asset Account",
                BankId = Bank.KiwibankId,
                AccountTypeId = AccountConstants.Asset,
                CurrencyId = Currency.NZD,
                AccountNumber = "1",
                IsActive = true
            },
            new Account
            {
                Id = Guid.NewGuid(),
                Name = "Revenue Account",
                BankId = Bank.KiwibankId,
                AccountTypeId = AccountConstants.Revenue,
                CurrencyId = Currency.NZD,
                AccountNumber = "2",
                IsActive = false
            }
        });
        await repository.UpsertEntitiesAsync(_accounts, CancellationToken.None);
    }

}

