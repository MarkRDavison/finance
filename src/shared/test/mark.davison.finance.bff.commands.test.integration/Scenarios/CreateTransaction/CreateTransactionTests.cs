namespace mark.davison.finance.bff.commands.test.integration.Scenarios.CreateTransaction;

[TestClass]
public class CreateTransactionTests : CommandIntegrationTestBase
{
    private readonly Guid _assetAccountId = new Guid("704f2e50-51e4-45a6-b05e-7f50df710d44");
    private readonly Guid _revenueAccountId = new Guid("ee97f134-e098-47f1-a6f5-200b3d558cd9");

    protected override async Task SeedData(IRepository repository)
    {
        await base.SeedData(repository);

        await repository.UpsertEntitiesAsync(new List<Account> {
            new Account
            {
                Id = _assetAccountId,
                AccountTypeId = AccountConstants.Asset,
                BankId = Bank.KiwibankId,
                CurrencyId = Currency.NZD
            },
            new Account
            {
                Id = _revenueAccountId,
                AccountTypeId = AccountConstants.Revenue,
                BankId = Bank.KiwibankId,
                CurrencyId = Currency.NZD
            }
        }, CancellationToken.None);
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
                    SourceAccountId = _revenueAccountId,
                    DestinationAccountId = _assetAccountId,
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
