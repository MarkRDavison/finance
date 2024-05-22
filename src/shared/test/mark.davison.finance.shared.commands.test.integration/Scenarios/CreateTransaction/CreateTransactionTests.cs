namespace mark.davison.finance.shared.commands.test.integration.Scenarios.CreateTransaction;

[TestClass]
public class CreateTransactionTests : CQRSIntegrationTestBase
{
    protected override async Task SeedTestData()
    {
        await GetRequiredService<AccountSeeder>().CreateStandardAccounts();
    }

    [TestMethod]
    public async Task SimpleDepositWorks()
    {
        var handler = GetRequiredService<ICommandHandler<CreateTransactionRequest, CreateTransactionResponse>>();
        var currentUserContext = GetRequiredService<ICurrentUserContext>();

        var request = new CreateTransactionRequest
        {
            TransactionTypeId = TransactionConstants.Deposit,
            Transactions =
            {
                new CreateTransactionDto
                {
                    SourceAccountId = AccountTestConstants.RevenueAccount1Id,
                    DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                    CurrencyId = Currency.NZD,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow)
                }
            }
        };

        var response = await handler.Handle(request, currentUserContext, CancellationToken.None);

        Assert.IsTrue(response.Success);
        Assert.IsNotNull(response.Group);
        Assert.AreEqual(2, response.Transactions.Count);
        Assert.AreEqual(1, response.Journals.Count);
    }
}
