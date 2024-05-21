namespace mark.davison.finance.models.EntityConfiguration;

public partial class AccountTypeEntityConfiguration : FinanceEntityConfiguration<AccountType>
{
    public override void ConfigureEntity(EntityTypeBuilder<AccountType> builder)
    {
        builder
            .Property(_ => _.Type)
            .HasMaxLength(127);

        builder.HasData(
            new AccountType { Id = AccountTypeConstants.Default, Type = "Default", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Cash, Type = "Cash", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Asset, Type = "Asset", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Expense, Type = "Expense", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Revenue, Type = "Revenue", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.InitialBalance, Type = "Initial balance", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Beneficiary, Type = "Beneficiary", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Import, Type = "Import", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Loan, Type = "Loan", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Reconciliation, Type = "Reconcilation", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Debt, Type = "Debt", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.Mortgage, Type = "Mortgage", UserId = Guid.Empty },
            new AccountType { Id = AccountTypeConstants.LiabilityCredit, Type = "Liability credit", UserId = Guid.Empty }
        );
    }
}

