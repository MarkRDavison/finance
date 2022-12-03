namespace mark.davison.finance.bff.Authentication;

public class FinanceCustomZenoAuthenticationActions : ICustomZenoAuthenticationActions
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpRepository _httpRepository;
    private readonly IOptions<AppSettings> _appSettings;

    public FinanceCustomZenoAuthenticationActions(
        IServiceProvider serviceProvider,
        IHttpRepository httpRepository,
        IOptions<AppSettings> appSettings
    )
    {
        _serviceProvider = serviceProvider;
        _httpRepository = httpRepository;
        _appSettings = appSettings;
    }

    private Task<User?> GetUser(Guid sub, CancellationToken cancellationToken)
    {
        return _httpRepository.GetEntityAsync<User>(
               new QueryParameters { { nameof(User.Sub), sub.ToString() } },
               HeaderParameters.None,
               cancellationToken);
    }

    private Task<User?> UpsertUser(UserProfile userProfile, string token, CancellationToken cancellationToken)
    {
        return _httpRepository.UpsertEntityAsync(
                new User
                {
                    Id = Guid.NewGuid(),
                    Sub = userProfile.sub,
                    Admin = false,
                    Created = DateTime.UtcNow,
                    Email = userProfile.email!,
                    First = userProfile.given_name!,
                    Last = userProfile.family_name!,
                    LastModified = DateTime.UtcNow,
                    Username = userProfile.preferred_username!
                },
                HeaderParameters.Auth(token, null),
                cancellationToken);
    }

    private async Task UpsertTestData(User currentUser, string token, CancellationToken cancellationToken)
    {
        var header = HeaderParameters.Auth(token, null);
        var assetAccount1 = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountNumber = "Asset 1", AccountTypeId = AccountConstants.Asset, CurrencyId = Currency.NZD, Name = "Test Asset 1" };
        var assetAccount2 = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountNumber = "Asset 2", AccountTypeId = AccountConstants.Asset, CurrencyId = Currency.NZD, Name = "Test Asset 2" };
        var revenueAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountNumber = "Revenue 1", AccountTypeId = AccountConstants.Revenue, CurrencyId = Currency.NZD, Name = "Test Revenue 1" };
        var expenseAccount1 = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountNumber = "Expense 1", AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Test Expense 1" };
        var expenseAccount2 = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountNumber = "Expense 2", AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Test Expense 2" };
        var expenseAccount3 = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountNumber = "Expense 3", AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Test Expense 3" };

        await _httpRepository.UpsertEntitiesAsync(new[] { assetAccount1, assetAccount2, revenueAccount, expenseAccount1, expenseAccount2, expenseAccount3 }.ToList(), header, cancellationToken);

        var transactionGroups = new List<TransactionGroup>();
        var transactionJournals = new List<TransactionJournal>();
        var transactions = new List<Transaction>();

        var createTransaction = (Guid transactionTypeId, decimal amount, Guid sourceAccountId, Guid destinationAccountId, string description, DateOnly date) =>
        {
            var transactionGroup = new TransactionGroup { Id = Guid.NewGuid(), UserId = currentUser.Id };
            var transactionJournal = new TransactionJournal { Id = Guid.NewGuid(), UserId = currentUser.Id, TransactionGroupId = transactionGroup.Id, TransactionTypeId = transactionTypeId, CurrencyId = Currency.NZD, Description = description, Date = date };
            var sourceTransaction = new Transaction { Id = Guid.NewGuid(), UserId = currentUser.Id, CurrencyId = Currency.NZD, TransactionJournalId = transactionJournal.Id, Amount = -CurrencyRules.ToPersisted(amount), AccountId = sourceAccountId, Description = description };
            var destTransaction = new Transaction { Id = Guid.NewGuid(), UserId = currentUser.Id, CurrencyId = Currency.NZD, TransactionJournalId = transactionJournal.Id, Amount = +CurrencyRules.ToPersisted(amount), AccountId = destinationAccountId, Description = description };

            transactionGroups.Add(transactionGroup);
            transactionJournals.Add(transactionJournal);
            transactions.Add(sourceTransaction);
            transactions.Add(destTransaction);
        };

        createTransaction(TransactionConstants.OpeningBalance, 100.0M, Account.OpeningBalance, assetAccount1.Id, "Opening balance", new DateOnly(2022, 1, 15));
        createTransaction(TransactionConstants.OpeningBalance, 1500.0M, Account.OpeningBalance, assetAccount2.Id, "Opening balance", new DateOnly(2022, 2, 5));
        createTransaction(TransactionConstants.Deposit, 140.0M, revenueAccount.Id, assetAccount1.Id, "Refund", new DateOnly(2022, 2, 11));
        createTransaction(TransactionConstants.Deposit, 120.0M, revenueAccount.Id, assetAccount1.Id, "Misc deposit", new DateOnly(2022, 3, 7));
        createTransaction(TransactionConstants.Transfer, 40.0M, assetAccount1.Id, assetAccount2.Id, "Transfer gas money", new DateOnly(2022, 4, 3));

        await _httpRepository.UpsertEntitiesAsync(transactionGroups, header, cancellationToken);
        await _httpRepository.UpsertEntitiesAsync(transactionJournals, header, cancellationToken);
        await _httpRepository.UpsertEntitiesAsync(transactions, header, cancellationToken);
    }

    public async Task OnUserAuthenticated(UserProfile userProfile, IZenoAuthenticationSession zenoAuthenticationSession, CancellationToken cancellationToken)
    {
        var token = zenoAuthenticationSession.GetString(ZenoAuthenticationConstants.SessionNames.AccessToken);
        var user = await GetUser(userProfile.sub, cancellationToken);

        if (user == null && !string.IsNullOrEmpty(token))
        {
            user = await UpsertUser(userProfile, token, cancellationToken);

            if (!_appSettings.Value.PRODUCTION_MODE && user != null)
            {
                await UpsertTestData(user, token, cancellationToken);
            }
        }

        if (user != null)
        {
            zenoAuthenticationSession.SetString(ZenoAuthenticationConstants.SessionNames.User, JsonSerializer.Serialize(user));
        }
    }
}
