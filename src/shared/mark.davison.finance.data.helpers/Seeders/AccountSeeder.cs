using mark.davison.finance.accounting.rules;

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
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<UpsertAccountCommandRequest, UpsertAccountCommandResponse>>();
        var currentUserContext = _serviceProvider.GetRequiredService<ICurrentUserContext>();

        return handler.Handle(new() { UpsertAccountDto = account }, currentUserContext, CancellationToken.None);
    }

    public async Task CreateAccount(params UpsertAccountDto[] accounts)
    {
        foreach (var account in accounts)
        {
            var result = await CreateAccount(account);
            if (!result.Success)
            {
                throw new InvalidOperationException("CreateAccount failed: " + string.Join(", ", result.Errors));
            }
        }
    }

    public async Task CreateStandardAccounts(DateOnly openingBalanceDate)
    {
        var assetAccount1 = new UpsertAccountDto
        {
            Id = AccountTestConstants.AssetAccount1Id,
            AccountNumber = AccountTestConstants.AssetAccount1AccountNumber,
            AccountTypeId = AccountTypeConstants.Asset,
            CurrencyId = Currency.NZD,
            Name = AccountTestConstants.AssetAccount1Name,
            OpeningBalanceDate = openingBalanceDate,
            OpeningBalance = CurrencyRules.ToPersisted(400.0M)
        };
        var assetAccount2 = new UpsertAccountDto
        {
            Id = AccountTestConstants.AssetAccount2Id,
            AccountNumber = AccountTestConstants.AssetAccount2AccountNumber,
            AccountTypeId = AccountTypeConstants.Asset,
            CurrencyId = Currency.NZD,
            Name = AccountTestConstants.AssetAccount2Name,
            OpeningBalanceDate = openingBalanceDate,
            OpeningBalance = CurrencyRules.ToPersisted(50.0M)
        };
        var revenueAccount1 = new UpsertAccountDto
        {
            Id = AccountTestConstants.RevenueAccount1Id,
            AccountNumber = AccountTestConstants.RevenueAccount1AccountNumber,
            AccountTypeId = AccountTypeConstants.Revenue,
            CurrencyId = Currency.NZD,
            Name = AccountTestConstants.RevenueAccount1Name
        };
        var revenueAccount2 = new UpsertAccountDto
        {
            Id = AccountTestConstants.RevenueAccount2Id,
            AccountNumber = AccountTestConstants.RevenueAccount2AccountNumber,
            AccountTypeId = AccountTypeConstants.Revenue,
            CurrencyId = Currency.NZD,
            Name = AccountTestConstants.RevenueAccount2Name
        };
        var expenseAccount1 = new UpsertAccountDto
        {
            Id = AccountTestConstants.ExpenseAccount1Id,
            AccountNumber = AccountTestConstants.ExpenseAccount1AccountNumber,
            AccountTypeId = AccountTypeConstants.Expense,
            CurrencyId = Currency.NZD,
            Name = AccountTestConstants.ExpenseAccount1Name
        };
        var expenseAccount2 = new UpsertAccountDto
        {
            Id = AccountTestConstants.ExpenseAccount2Id,
            AccountNumber = AccountTestConstants.ExpenseAccount2AccountNumber,
            AccountTypeId = AccountTypeConstants.Expense,
            CurrencyId = Currency.NZD,
            Name = AccountTestConstants.ExpenseAccount2Name
        };
        var expenseAccount3 = new UpsertAccountDto
        {
            Id = AccountTestConstants.ExpenseAccount3Id,
            AccountNumber = AccountTestConstants.ExpenseAccount3AccountNumber,
            AccountTypeId = AccountTypeConstants.Expense,
            CurrencyId = Currency.NZD,
            Name = AccountTestConstants.ExpenseAccount3Name
        };

        await CreateAccount(assetAccount1, assetAccount2, revenueAccount1, revenueAccount2, expenseAccount1, expenseAccount2, expenseAccount3);
    }

}
