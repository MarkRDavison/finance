namespace mark.davison.finance.data.helpers.Seeders;

public class AccountSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public AccountSeeder(
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
    }

    public Task<UpsertAccountCommandResponse> CreateAccount(UpsertAccountDto account)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>>();
        var currentUserContext = scope.ServiceProvider.GetRequiredService<ICurrentUserContext>();

        return handler.Handle(new() { UpsertAccountDto = account }, currentUserContext, CancellationToken.None);
    }

    public async Task CreateAccount(params UpsertAccountDto[] accounts)
    {
        foreach (var account in accounts)
        {
            var result = await CreateAccount(account);
            if (!result.Success)
            {
                throw new InvalidOperationException("CreateAccount failed.");
            }
        }
    }

    public async Task CreateStandardAccounts()
    {
        var assetAccount1 = new UpsertAccountDto
        {
            Id = AccountTestConstants.AssetAccount1Id,
            AccountNumber = "Asset 1",
            AccountTypeId = AccountConstants.Asset,
            CurrencyId = Currency.NZD,
            Name = "Test Asset 1"
        };
        var assetAccount2 = new UpsertAccountDto
        {
            Id = AccountTestConstants.AssetAccount2Id,
            AccountNumber = "Asset 2",
            AccountTypeId = AccountConstants.Asset,
            CurrencyId = Currency.NZD,
            Name = "Test Asset 2"
        };
        var revenueAccount = new UpsertAccountDto
        {
            Id = AccountTestConstants.RevenueAccount1Id,
            AccountNumber = "Revenue 1",
            AccountTypeId = AccountConstants.Revenue,
            CurrencyId = Currency.NZD,
            Name = "Test Revenue 1"
        };
        var expenseAccount1 = new UpsertAccountDto
        {
            Id = AccountTestConstants.ExpenseAccount1Id,
            AccountNumber = "Expense 1",
            AccountTypeId = AccountConstants.Expense,
            CurrencyId = Currency.NZD,
            Name = "Test Expense 1"
        };
        var expenseAccount2 = new UpsertAccountDto
        {
            Id = AccountTestConstants.ExpenseAccount2Id,
            AccountNumber = "Expense 2",
            AccountTypeId = AccountConstants.Expense,
            CurrencyId = Currency.NZD,
            Name = "Test Expense 2"
        };
        var expenseAccount3 = new UpsertAccountDto
        {
            Id = AccountTestConstants.ExpenseAccount3Id,
            AccountNumber = "Expense 3",
            AccountTypeId = AccountConstants.Expense,
            CurrencyId = Currency.NZD,
            Name = "Test Expense 3"
        };

        await CreateAccount(assetAccount1, assetAccount2, revenueAccount, expenseAccount1, expenseAccount2, expenseAccount3);
    }

}
