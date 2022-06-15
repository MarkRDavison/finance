namespace mark.davison.finance.models.EntityConfiguration;

public partial class CurrencyExchangeRateEntityConfiguration : FinanceEntityConfiguration<CurrencyExchangeRate>
{
    public override void ConfigureEntity(EntityTypeBuilder<CurrencyExchangeRate> builder)
    {
        ConfigureConversion(builder
            .Property(_ => _.Date));

        builder
            .Property(_ => _.Rate);

        builder
            .Property(_ => _.UserRate);
    }
}

