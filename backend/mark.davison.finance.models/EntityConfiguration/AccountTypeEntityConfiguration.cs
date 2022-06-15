namespace mark.davison.finance.models.EntityConfiguration;

public partial class AccountTypeEntityConfiguration : FinanceEntityConfiguration<AccountType>
{
    public override void ConfigureEntity(EntityTypeBuilder<AccountType> builder)
    {
        builder
            .Property(_ => _.Type)
            .HasMaxLength(127);
    }
}

