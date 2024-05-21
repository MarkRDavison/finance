namespace mark.davison.finance.models.EntityConfiguration;

public partial class AccountEntityConfiguration : FinanceEntityConfiguration<Account>
{
    public override void ConfigureEntity(EntityTypeBuilder<Account> builder)
    {
        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.IsActive);

        builder
            .Property(_ => _.VirtualBalance);

        builder
            .Property(_ => _.AccountNumber)
            .HasMaxLength(255);

        builder
            .Property(_ => _.Order);

        builder.HasData(
            new Account { Id = AccountConstants.OpeningBalance, UserId = Guid.Empty, AccountTypeId = AccountTypeConstants.InitialBalance, CurrencyId = Currency.INT, Name = "Opening balance" },
            new Account { Id = AccountConstants.Reconciliation, UserId = Guid.Empty, AccountTypeId = AccountTypeConstants.Reconciliation, CurrencyId = Currency.INT, Name = "Reconcilation" }
        );
    }
}

