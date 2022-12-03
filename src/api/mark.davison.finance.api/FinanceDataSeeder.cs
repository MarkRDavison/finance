namespace mark.davison.finance.api;

public interface IFinanceDataSeeder
{
    Task EnsureDataSeeded(CancellationToken cancellationToken);
}

public class FinanceDataSeeder : IFinanceDataSeeder
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly AppSettings _appSettings;

    public FinanceDataSeeder(
        IServiceProvider serviceProvider,
        IOptions<AppSettings> options
    )
    {
        _serviceProvider = serviceProvider;
        _appSettings = options.Value;
    }

    public virtual async Task EnsureDataSeeded(CancellationToken cancellationToken)
    {
        await EnsureUserSeeded(cancellationToken);
        await EnsureAccountTypesSeeded(cancellationToken);
        await EnsureLinkTypesSeeded(cancellationToken);
        await EnsureTransactionTypesSeeded(cancellationToken);
        await EnsureCurrenciesSeeded(cancellationToken);
        await EnsureCoreAccountsSeeded(cancellationToken);
    }

    internal async Task EnsureSeeded<T>(List<T> entities, CancellationToken cancellationToken)
        where T : FinanceEntity
    {
        var repository = _serviceProvider.GetRequiredService<IRepository>();
        var existingEntities = await repository.GetEntitiesAsync<T>(_ => _.UserId == Guid.Empty, cancellationToken);

        var newEntities = entities.Where(_ => !existingEntities.Any(e => e.Id == _.Id)).ToList();

        await repository.UpsertEntitiesAsync(newEntities, cancellationToken);
    }

    private async Task EnsureUserSeeded(CancellationToken cancellationToken)
    {
        var repository = _serviceProvider.GetRequiredService<IRepository>();
        var seededUser = new User { Id = Guid.Empty, Email = "financesystem@markdavison.kiwi", First = "Finance", Last = "System", Username = "Finance.System" };

        var existingUser = await repository.GetEntityAsync<User>(_ => _.Id == Guid.Empty, cancellationToken);

        if (existingUser == null)
        {
            await repository.UpsertEntityAsync(seededUser, cancellationToken);
        }

    }

    private async Task EnsureAccountTypesSeeded(CancellationToken cancellationToken)
    {
        var seededAccountTypes = new List<AccountType>
        {
            new AccountType{ Id = AccountConstants.Default, Type = "Default", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Cash, Type = "Cash", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Asset, Type = "Asset", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Expense, Type = "Expense", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Revenue, Type = "Revenue", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.InitialBalance, Type = "Initial balance", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Beneficiary, Type = "Beneficiary", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Import, Type = "Import", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Loan, Type = "Loan", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Reconciliation, Type = "Reconcilation", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Debt, Type = "Debt", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.Mortgage, Type = "Mortgage", UserId = Guid.Empty },
            new AccountType{ Id = AccountConstants.LiabilityCredit, Type = "Liability credit", UserId = Guid.Empty },
        };

        await EnsureSeeded(seededAccountTypes, cancellationToken);
    }
    private async Task EnsureLinkTypesSeeded(CancellationToken cancellationToken)
    {
        var seededLinkTypes = new List<LinkType>
        {
            new LinkType{ Id = LinkType.Related, Name = "Related", Inward = "relates to", Outward = "relates to", Editable = false, UserId= Guid.Empty },
            new LinkType{ Id = LinkType.Refund, Name = "Refund", Inward = "is (partially) refunded by", Outward = "(partially) refunds", Editable = false, UserId= Guid.Empty },
            new LinkType{ Id = LinkType.Paid, Name = "Paid", Inward = "is (partially) paid for by", Outward = "(partially) pays for", Editable = false, UserId= Guid.Empty },
            new LinkType{ Id = LinkType.Reimbursement, Name = "Reimbursement", Inward = "is (partially) reimbursed by", Outward = "(partially) reimburses", Editable = false, UserId= Guid.Empty },
        };
        await EnsureSeeded(seededLinkTypes, cancellationToken);
    }
    private async Task EnsureTransactionTypesSeeded(CancellationToken cancellationToken)
    {
        var seededTransactionTypes = new List<TransactionType>
        {
            new TransactionType{ Id = TransactionConstants.Withdrawal, Type = "Withdrawal", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionConstants.Deposit, Type = "Deposit", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionConstants.Transfer, Type = "Transfer", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionConstants.OpeningBalance, Type = "Opening balance", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionConstants.Reconciliation, Type = "Reconciliation", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionConstants.Invalid, Type = "Invalid", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionConstants.LiabilityCredit, Type = "Liability credit", UserId = Guid.Empty },
        };
        await EnsureSeeded(seededTransactionTypes, cancellationToken);
    }

    private async Task EnsureCurrenciesSeeded(CancellationToken cancellationToken)
    {
        var seededCurrencies = new List<Currency>
        {
            new Currency { Id = Currency.NZD, Code = "NZD", Name = "New Zealand Dollar", Symbol = "NZ$", DecimalPlaces = 2  },
            new Currency { Id = Currency.AUD, Code = "AUD", Name = "Australian Dollar", Symbol = "A$", DecimalPlaces = 2  },
            new Currency { Id = Currency.USD, Code = "USD", Name = "US  Dollar", Symbol = "US$", DecimalPlaces = 2  },
            new Currency { Id = Currency.CAD, Code = "CAD", Name = "Canadian Dollar", Symbol = "C$", DecimalPlaces = 2  },
            new Currency { Id = Currency.EUR, Code = "EUR", Name = "Euro", Symbol = "€", DecimalPlaces = 2  },
            new Currency { Id = Currency.GBP, Code = "GBP", Name = "British Pound", Symbol = "£", DecimalPlaces = 2  },
            new Currency { Id = Currency.JPY, Code = "JPY", Name = "Japanese Yen", Symbol = "¥", DecimalPlaces = 0  },
            new Currency { Id = Currency.RMB, Code = "RMB", Name = "Chinese Yuan", Symbol = "¥", DecimalPlaces = 2  },
            new Currency { Id = Currency.INT, Code = "INT", Name = "Internal", Symbol = "$", DecimalPlaces = 2  },
        };
        await EnsureSeeded(seededCurrencies, cancellationToken);
    }

    private async Task EnsureCoreAccountsSeeded(CancellationToken cancellationToken)
    {
        var seededAccounts = new List<Account>
        {
            new Account { Id = Account.OpeningBalance, UserId = Guid.Empty, AccountTypeId = AccountConstants.InitialBalance, CurrencyId = Currency.INT, Name = "Opening balance" },
            new Account { Id = Account.Reconciliation, UserId = Guid.Empty, AccountTypeId = AccountConstants.Reconciliation, CurrencyId = Currency.INT, Name = "Reconcilation" }
        };
        await EnsureSeeded(seededAccounts, cancellationToken);
    }
}
