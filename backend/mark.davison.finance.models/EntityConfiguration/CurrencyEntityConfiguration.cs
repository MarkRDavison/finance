namespace mark.davison.finance.models.EntityConfiguration;

public partial class CurrencyEntityConfiguration : FinanceEntityConfiguration<Currency>
{
    public override void ConfigureEntity(EntityTypeBuilder<Currency> builder)
    {
        builder
            .Property(_ => _.Code)
            .HasMaxLength(3);

        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.Symbol)
            .HasMaxLength(4);

        builder
            .Property(_ => _.DecimalPlaces);

        builder
            .Property(_ => _.IsActive);
    }
}

