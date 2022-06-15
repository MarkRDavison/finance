using mark.davison.finance.common.server.Repository;

namespace mark.davison.finance.api;

public interface IFinanceDataSeeder
{
    Task EnsureDataSeeded(CancellationToken cancellationToken);
}

public class FinanceDataSeeder : IFinanceDataSeeder
{
    private readonly IRepository _repository;

    public FinanceDataSeeder(IRepository repository)
    {
        _repository = repository;
    }

    public async Task EnsureDataSeeded(CancellationToken cancellationToken)
    {
        await EnsureUserSeeded(cancellationToken);
        await EnsureBanksSeeded(cancellationToken);
        await EnsureAccountTypesSeeded(cancellationToken);
        await EnsureLinkTypesSeeded(cancellationToken);
        await EnsureTransactionTypesSeeded(cancellationToken);
        await EnsureCurrenciesSeeded(cancellationToken);
    }

    internal async Task EnsureSeeded<T>(List<T> entities, CancellationToken cancellationToken)
        where T : FinanceEntity
    {
        var existingEntities = await _repository.GetEntitiesAsync<T>(_ => _.UserId == Guid.Empty, cancellationToken);

        var newEntities = entities.Where(_ => !existingEntities.Any(e => e.Id == _.Id)).ToList();

        await _repository.UpsertEntitiesAsync(newEntities, cancellationToken);
    }

    private async Task EnsureUserSeeded(CancellationToken cancellationToken)
    {
        var seededUser = new User { Id = Guid.Empty, Email = "financesystem@markdavison.kiwi", First = "Finance", Last = "System", Username = "Finance.System" };

        var existingUser = await _repository.GetEntityAsync<User>(_ => _.Id == Guid.Empty, cancellationToken);

        if (existingUser == null)
        {
            await _repository.UpsertEntityAsync(seededUser, cancellationToken);
        }

    }
    private async Task EnsureBanksSeeded(CancellationToken cancellationToken)
    {
        var seededBanks = new List<Bank>
        {
            new Bank{ Id = Bank.KiwibankId, Name = "Kiwibank", UserId = Guid.Empty },
            new Bank{ Id = Bank.BnzId, Name = "BNZ", UserId = Guid.Empty }
        };

        await EnsureSeeded(seededBanks, cancellationToken);
    }

    private async Task EnsureAccountTypesSeeded(CancellationToken cancellationToken)
    {
        var seededAccountTypes = new List<AccountType>
        {
            new AccountType{ Id = AccountType.Default, Type = "Default", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Cash, Type = "Cash", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Asset, Type = "Asset", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Expense, Type = "Expense", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Revenue, Type = "Revenue", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.InitialBalance, Type = "Initial balance", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Beneficiary, Type = "Beneficiary", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Import, Type = "Import", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Loan, Type = "Loan", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Reconcilation, Type = "Reconcilation", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Debt, Type = "Debt", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.Mortgage, Type = "Mortgage", UserId = Guid.Empty },
            new AccountType{ Id = AccountType.LiabilityCredit, Type = "Liability credit", UserId = Guid.Empty },
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
            new TransactionType{ Id = TransactionType.Withdrawal, Type = "Withdrawal", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionType.Deposit, Type = "Deposit", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionType.Transfer, Type = "Transfer", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionType.OpeningBalance, Type = "Opening balance", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionType.Reconciliation, Type = "Reconciliation", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionType.Invalid, Type = "Invalid", UserId = Guid.Empty },
            new TransactionType{ Id = TransactionType.LiabilityCredit, Type = "Liability credit", UserId = Guid.Empty },
        };
        await EnsureSeeded(seededTransactionTypes, cancellationToken);
    }
    private async Task EnsureCurrenciesSeeded(CancellationToken cancellationToken)
    {
        var seededCurrencies = new List<Currency>
        {
            new Currency { Id = Currency.NZD, Code = "NZD", Name = "New Zealand Dollar", Symbol = "$", DecimalPlaces = 2  },
            new Currency { Id = Currency.AUD, Code = "AUD", Name = "Australian Dollar", Symbol = "A$", DecimalPlaces = 2  },
            new Currency { Id = Currency.USD, Code = "USD", Name = "US  Dollar", Symbol = "US$", DecimalPlaces = 2  },
            new Currency { Id = Currency.CAD, Code = "CAD", Name = "Canadian Dollar", Symbol = "C$", DecimalPlaces = 2  },
            new Currency { Id = Currency.EUR, Code = "EUR", Name = "Euro", Symbol = "€", DecimalPlaces = 2  },
            new Currency { Id = Currency.GBP, Code = "GBP", Name = "British Pound", Symbol = "£", DecimalPlaces = 2  },
            new Currency { Id = Currency.JPY, Code = "JPY", Name = "Japanese Yen", Symbol = "¥", DecimalPlaces = 0  },
            new Currency { Id = Currency.RMB, Code = "RMB", Name = "Chinese Yuan", Symbol = "¥", DecimalPlaces = 2  },
        };
        await EnsureSeeded(seededCurrencies, cancellationToken);
    }
}
