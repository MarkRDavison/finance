using mark.davison.finance.accounting.rules.Account;

namespace mark.davison.finance.bff.Authentication;

public class FinanceCustomZenoAuthenticationActions : ICustomZenoAuthenticationActions
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpRepository _httpRepository;
    private readonly IDateService _dateService;
    private readonly IOptions<AppSettings> _appSettings;

    public FinanceCustomZenoAuthenticationActions(
        IServiceProvider serviceProvider,
        IHttpRepository httpRepository,
        IDateService dateService,
        IOptions<AppSettings> appSettings
    )
    {
        _serviceProvider = serviceProvider;
        _httpRepository = httpRepository;
        _dateService = dateService;
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
                    Created = _dateService.Now,
                    Email = userProfile.email!,
                    First = userProfile.given_name!,
                    Last = userProfile.family_name!,
                    LastModified = _dateService.Now,
                    Username = userProfile.preferred_username!
                },
                HeaderParameters.Auth(token, null),
                cancellationToken);
    }

    // TODO: Make api endpoint to do this and remove the random crud api endpoints. or replace with cqrs calls
    private async Task UpsertTestData(User currentUser, string token, CancellationToken cancellationToken)
    {
        var header = HeaderParameters.Auth(token, currentUser);
        var everydayAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Asset, CurrencyId = Currency.NZD, Name = "Everyday account" };
        var savingsAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Asset, CurrencyId = Currency.NZD, Name = "Savings account" };
        var jobAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Revenue, CurrencyId = Currency.NZD, Name = "Job" };
        var groceryStoreAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Grocery store" };
        var gasStationAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Gas station" };
        var powerGasInternetAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Power, Gas, Internet" };
        var waterAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Water" };
        var taxAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Taxes" };
        var mechanicAccount = new Account { Id = Guid.NewGuid(), UserId = currentUser.Id, AccountTypeId = AccountConstants.Expense, CurrencyId = Currency.NZD, Name = "Mechanic" };

        var transactionGroups = new List<TransactionGroup>();
        var transactionJournals = new List<TransactionJournal>();
        var transactions = new List<Transaction>();

        var createTransaction = (Guid transactionTypeId, decimal amount, Guid sourceAccountId, Guid destinationAccountId, string description, DateOnly date) =>
        {
            var transactionGroup = new TransactionGroup { Id = Guid.NewGuid(), UserId = currentUser.Id };
            var transactionJournal = new TransactionJournal { Id = Guid.NewGuid(), UserId = currentUser.Id, TransactionGroupId = transactionGroup.Id, TransactionTypeId = transactionTypeId, CurrencyId = Currency.NZD, Description = description, Date = date };
            var sourceTransaction = new Transaction { Id = Guid.NewGuid(), UserId = currentUser.Id, CurrencyId = Currency.NZD, TransactionJournalId = transactionJournal.Id, Amount = -CurrencyRules.ToPersisted(amount), AccountId = sourceAccountId, Description = description, IsSource = true };
            var destTransaction = new Transaction { Id = Guid.NewGuid(), UserId = currentUser.Id, CurrencyId = Currency.NZD, TransactionJournalId = transactionJournal.Id, Amount = +CurrencyRules.ToPersisted(amount), AccountId = destinationAccountId, Description = description, IsSource = false };

            transactionGroups.Add(transactionGroup);
            transactionJournals.Add(transactionJournal);
            transactions.Add(sourceTransaction);
            transactions.Add(destTransaction);
        };

        var createStandardMonthlyTransactions = (int year, int month) =>
        {
            createTransaction(TransactionConstants.Deposit, 1000.0M, jobAccount.Id, everydayAccount.Id, "Salary", new DateOnly(year, month, 11));
            createTransaction(TransactionConstants.Transfer, 400.0M, everydayAccount.Id, savingsAccount.Id, "Transfer savings", new DateOnly(year, month, 15));
            createTransaction(TransactionConstants.Withdrawal, 170.0M, everydayAccount.Id, powerGasInternetAccount.Id, "Monthly utilities", new DateOnly(year, month, 19));
            createTransaction(TransactionConstants.Withdrawal, 62.0M, everydayAccount.Id, waterAccount.Id, "Monthly water", new DateOnly(year, month, 13));
            createTransaction(TransactionConstants.Withdrawal, 231.0M, everydayAccount.Id, taxAccount.Id, "Taxes", new DateOnly(year, month, 8));
        };


        bool openingBalancesMade = false;
        foreach (var year in new[] { 2020, 2021, 2022, 2023 })
        {
            if (!openingBalancesMade)
            {
                openingBalancesMade = true;
                createTransaction(TransactionConstants.OpeningBalance, 100.0M, BuiltinAccountNames.OpeningBalance, everydayAccount.Id, "Opening balance", new DateOnly(year, 1, 1));
                createTransaction(TransactionConstants.OpeningBalance, 1500.0M, BuiltinAccountNames.OpeningBalance, savingsAccount.Id, "Opening balance", new DateOnly(year, 1, 1));
            }
            for (int month = 1; month <= 12; ++month)
            {
                createStandardMonthlyTransactions(year, month);
            }
        }

        createTransaction(TransactionConstants.Withdrawal, 585.0M, everydayAccount.Id, mechanicAccount.Id, "Repairs", new DateOnly(2022, 4, 21));

        await _httpRepository.UpsertEntitiesAsync(new[] { everydayAccount, savingsAccount, jobAccount, groceryStoreAccount, gasStationAccount, powerGasInternetAccount, waterAccount, taxAccount, mechanicAccount }.ToList(), header, cancellationToken);
        await _httpRepository.UpsertEntitiesAsync(transactionGroups, header, cancellationToken);
        await _httpRepository.UpsertEntitiesAsync(transactionJournals, header, cancellationToken);
        await _httpRepository.UpsertEntitiesAsync(transactions, header, cancellationToken);
    }

    public async Task<User?> OnUserAuthenticated(UserProfile userProfile, IZenoAuthenticationSession zenoAuthenticationSession, CancellationToken cancellationToken)
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

        return user;
    }
}
