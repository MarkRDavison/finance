﻿namespace mark.davison.finance.api.test.Controllers;

[TestClass]
public class TagControllerIntegrationTests : IntegrationTestBase<FinanceApiWebApplicationFactory, AppSettings>
{
    private readonly List<TransactionGroup> transactionGroups = new List<TransactionGroup>();
    private readonly List<TransactionJournal> transactionJournals = new List<TransactionJournal>();
    private readonly List<Transaction> transactions = new List<Transaction>();

    protected override async Task SeedData(IRepository repository)
    {
        var users = await repository.GetEntitiesAsync<User>();
        var currentUserId = users.First().Id;

        var accounts = FinanceDatatSeederHelpers.CreateStandardAccounts(currentUserId);

        var createTransaction = (Guid transactionTypeId, decimal amount, Guid sourceAccountId, Guid destinationAccountId, string description, DateOnly date, IEnumerable<string> tags) =>
        {
            var info = FinanceDatatSeederHelpers.CreateTransaction(currentUserId, transactionTypeId, amount, sourceAccountId, destinationAccountId, description, date, tags);

            transactionGroups.Add(info.TransactionGroup);
            transactionJournals.Add(info.TransactionJournal);
            transactions.Add(info.SourceTransaction);
            transactions.Add(info.DestinationTransaction);
        };

        createTransaction(TransactionConstants.OpeningBalance, 100.0M, Account.OpeningBalance, FinanceDatatSeederHelpers.AssetAccount1Id, "Opening balance", new DateOnly(2022, 1, 15), new List<string> { "Tag1", "Tag2" });

        await repository.UpsertEntitiesAsync(accounts, CancellationToken.None);
        await repository.UpsertEntitiesAsync(transactionGroups, CancellationToken.None);
        await repository.UpsertEntitiesAsync(transactionJournals, CancellationToken.None);
        await repository.UpsertEntitiesAsync(transactions, CancellationToken.None);
    }

    [TestMethod]
    public async Task QueryingTags_ReturnsTransactionJournals()
    {
        var tags = await GetMultipleAsync<Tag>("/api/Tag?include=TransactionJournals", true);

        Assert.AreEqual(2, tags.Count);
        Assert.IsNotNull(tags.First().TransactionJournals);
        Assert.AreEqual(1, tags.First().TransactionJournals!.Count);
    }
}
