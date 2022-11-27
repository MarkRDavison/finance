namespace mark.davison.finance.api.test.Framework;

public static class FinanceDatatSeederHelpers
{
    public class TransactionCreationInfo
    {
        public required TransactionGroup TransactionGroup { get; init; }
        public required TransactionJournal TransactionJournal { get; init; }
        public required Transaction SourceTransaction { get; init; }
        public required Transaction DestinationTransaction { get; init; }
    }

    private static IDictionary<string, Guid> TagIds = new Dictionary<string, Guid>();

    public static Guid AssetAccount1Id => new Guid("77123D1D-47C9-4627-8720-8CDEB297102C");
    public static Guid AssetAccount2Id => new Guid("A3D9AD8C-F3EF-4115-8C5B-45765F75F7A6");
    public static Guid RevenueAccount1Id => new Guid("33590C65-06D0-4479-A813-39E69CD223AE");
    public static Guid ExpenseAccount1Id => new Guid("2B055284-C252-4D26-ABC3-D8D3C8AFC159");
    public static Guid ExpenseAccount2Id => new Guid("298E723D-A664-4E71-A99F-8DA0F82C564E");
    public static Guid ExpenseAccount3Id => new Guid("42958ACF-0EB0-4210-896A-EEDAE67D853E");

    public static List<Account> CreateStandardAccounts(Guid currentUserId)
    {
        var assetAccount1 = new Account { Id = AssetAccount1Id, UserId = currentUserId, AccountNumber = "Asset 1", AccountTypeId = AccountConstants.Asset, CurrencyId = Currency.NZD, Name = "Test Asset 1" };
        var assetAccount2 = new Account { Id = AssetAccount2Id, UserId = currentUserId, AccountNumber = "Asset 2", AccountTypeId = AccountConstants.Asset, CurrencyId = Currency.NZD, Name = "Test Asset 2" };
        var revenueAccount = new Account { Id = RevenueAccount1Id, UserId = currentUserId, AccountNumber = "Revenue 1", AccountTypeId = AccountConstants.Revenue, CurrencyId = Currency.NZD, Name = "Test Revenue 1" };
        var expenseAccount1 = new Account { Id = ExpenseAccount1Id, UserId = currentUserId, AccountNumber = "Expense 1", AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Test Expense 1" };
        var expenseAccount2 = new Account { Id = ExpenseAccount2Id, UserId = currentUserId, AccountNumber = "Expense 2", AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Test Expense 2" };
        var expenseAccount3 = new Account { Id = ExpenseAccount3Id, UserId = currentUserId, AccountNumber = "Expense 3", AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Test Expense 3" };

        return new List<Account> { assetAccount1, assetAccount2, revenueAccount, expenseAccount1, expenseAccount2, expenseAccount3 };
    }

    public static TransactionCreationInfo CreateTransaction(Guid currentUserId, Guid transactionTypeId, decimal amount, Guid sourceAccountId, Guid destinationAccountId, string description, DateOnly date, IEnumerable<string> tags)
    {
        var transactionGroup = new TransactionGroup { Id = Guid.NewGuid(), UserId = currentUserId };
        var transactionJournal = new TransactionJournal { Id = Guid.NewGuid(), UserId = currentUserId, TransactionGroupId = transactionGroup.Id, TransactionTypeId = transactionTypeId, CurrencyId = Currency.NZD, Description = description, Date = date, Tags = new() };
        var sourceTransaction = new Transaction { Id = Guid.NewGuid(), UserId = currentUserId, CurrencyId = Currency.NZD, TransactionJournalId = transactionJournal.Id, Amount = -CurrencyRules.ToPersisted(amount), AccountId = sourceAccountId, Description = description };
        var destTransaction = new Transaction { Id = Guid.NewGuid(), UserId = currentUserId, CurrencyId = Currency.NZD, TransactionJournalId = transactionJournal.Id, Amount = +CurrencyRules.ToPersisted(amount), AccountId = destinationAccountId, Description = description };

        foreach (var t in tags)
        {
            if (!TagIds.ContainsKey(t))
            {
                var id = Guid.NewGuid();
                TagIds.Add(t, id);
            }

            transactionJournal.Tags.Add(new Tag { Id = TagIds[t], Name = t, UserId = currentUserId });
        }

        return new TransactionCreationInfo
        {
            TransactionGroup = transactionGroup,
            TransactionJournal = transactionJournal,
            SourceTransaction = sourceTransaction,
            DestinationTransaction = destTransaction
        };
    }

}
