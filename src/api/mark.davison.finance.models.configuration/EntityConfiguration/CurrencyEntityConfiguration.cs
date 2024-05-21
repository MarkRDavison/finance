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

        builder.HasData(
            new Currency { Id = Currency.NZD, Code = "NZD", Name = "New Zealand Dollar", Symbol = "NZ$", DecimalPlaces = 2 },
            new Currency { Id = Currency.AUD, Code = "AUD", Name = "Australian Dollar", Symbol = "A$", DecimalPlaces = 2 },
            new Currency { Id = Currency.USD, Code = "USD", Name = "US  Dollar", Symbol = "US$", DecimalPlaces = 2 },
            new Currency { Id = Currency.CAD, Code = "CAD", Name = "Canadian Dollar", Symbol = "C$", DecimalPlaces = 2 },
            new Currency { Id = Currency.EUR, Code = "EUR", Name = "Euro", Symbol = "€", DecimalPlaces = 2 },
            new Currency { Id = Currency.GBP, Code = "GBP", Name = "British Pound", Symbol = "£", DecimalPlaces = 2 },
            new Currency { Id = Currency.JPY, Code = "JPY", Name = "Japanese Yen", Symbol = "¥", DecimalPlaces = 0 },
            new Currency { Id = Currency.RMB, Code = "RMB", Name = "Chinese Yuan", Symbol = "¥", DecimalPlaces = 2 },
            new Currency { Id = Currency.INT, Code = "INT", Name = "Internal", Symbol = "$", DecimalPlaces = 2 }
        );
    }
}

