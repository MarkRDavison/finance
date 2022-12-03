namespace mark.davison.finance.api.test.Controllers;

[TestClass]
public class TransactionJournalControllerIntegrationTests : ApiIntegrationTestBase
{
    protected override async Task SeedTestData()
    {
        using var scope = Services.CreateScope();
        var repository = Services.GetRequiredService<IRepository>();
        var transactionSeeder = scope.ServiceProvider.GetRequiredService<TransactionSeeder>();

        await scope.ServiceProvider.GetRequiredService<AccountSeeder>().CreateStandardAccounts();
        await transactionSeeder.CreateTransaction(new CreateTransactionCommandRequest
        {
            TransactionTypeId = TransactionConstants.Deposit,
            Transactions =
            {
                new CreateTransactionDto
                {
                    Id = Guid.NewGuid(),
                    Amount = CurrencyRules.ToPersisted(100.0M),
                    SourceAccountId = AccountTestConstants.RevenueAccount1Id,
                    DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                    CurrencyId = Currency.NZD,
                    Date = new DateOnly(2022, 1, 15),
                    Description = "Example deposit",
                    Tags= { "Tag1", "Tag2" }
                }
            }
        });
    }

    [TestMethod]
    public async Task QueryingTransactionJournals_ReturnsTags()
    {
        var transactionJournals = await GetMultipleAsync<TransactionJournal>("/api/TransactionJournal?include=Tags", true);

        Assert.AreEqual(1, transactionJournals.Count);
        Assert.IsNotNull(transactionJournals.First().Tags);
        Assert.AreEqual(2, transactionJournals.First().Tags!.Count);
    }
}
