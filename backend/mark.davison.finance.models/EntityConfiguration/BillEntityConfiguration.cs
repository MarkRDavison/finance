namespace mark.davison.finance.models.EntityConfiguration;

public partial class BillEntityConfiguration : FinanceEntityConfiguration<Bill>
{
    public override void ConfigureEntity(EntityTypeBuilder<Bill> builder)
    {
        builder
            .Property(_ => _.IsActive);

        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.AmountMin);

        builder
            .Property(_ => _.AmountMax);

        ConfigureConversion(builder
            .Property(_ => _.Date));

        builder
            .Property(_ => _.AutoMatch);

        builder
            .Property(_ => _.Order);

        builder
            .Property(_ => _.RepeatFrequency)
            .HasMaxLength(63);

        ConfigureConversion(builder
            .Property(_ => _.EndDate));

        ConfigureConversion(builder
            .Property(_ => _.ExtensionDate));
    }
}

