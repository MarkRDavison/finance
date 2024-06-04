namespace mark.davison.finance.api.Seed;

public static class SeedDevData
{
    public static async Task SeedData(AccountSeeder accountSeeder, TransactionSeeder transactionSeeder)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        await accountSeeder.CreateStandardAccounts(monthStart.AddMonths(-1));
        await transactionSeeder.CreateTransaction(new()
        {
            TransactionTypeId = TransactionTypeConstants.Deposit,
            Transactions = [
                new CreateTransactionDto
                {
                    Id = Guid.NewGuid(),
                    Amount = CurrencyRules.ToPersisted(100.0M),
                    CurrencyId = Currency.NZD,
                    SourceAccountId =AccountTestConstants.RevenueAccount1Id,
                    DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                    Date = monthStart
                }
            ]
        });
        await transactionSeeder.CreateTransaction(new()
        {
            TransactionTypeId = TransactionTypeConstants.Deposit,
            Transactions = [
                new CreateTransactionDto
                {
                    Id = Guid.NewGuid(),
                    Amount = CurrencyRules.ToPersisted(80.0M),
                    CurrencyId = Currency.NZD,
                    SourceAccountId =AccountTestConstants.RevenueAccount1Id,
                    DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                    Date = monthStart.AddDays(2)
                }
            ]
        });
        await transactionSeeder.CreateTransaction(new()
        {
            TransactionTypeId = TransactionTypeConstants.Deposit,
            Transactions = [
                new CreateTransactionDto
                {
                    Id = Guid.NewGuid(),
                    Amount = CurrencyRules.ToPersisted(30.0M),
                    CurrencyId = Currency.NZD,
                    SourceAccountId =AccountTestConstants.RevenueAccount2Id,
                    DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                    Date = monthStart.AddDays(5)
                }
            ]
        });
        await transactionSeeder.CreateTransaction(new()
        {
            TransactionTypeId = TransactionTypeConstants.Deposit,
            Transactions = [
                new CreateTransactionDto
                {
                    Id = Guid.NewGuid(),
                    Amount = CurrencyRules.ToPersisted(100.0M),
                    CurrencyId = Currency.NZD,
                    SourceAccountId =AccountTestConstants.RevenueAccount2Id,
                    DestinationAccountId = AccountTestConstants.AssetAccount1Id,
                    Date = monthStart.AddDays(8)
                }
            ]
        });
    }
}
