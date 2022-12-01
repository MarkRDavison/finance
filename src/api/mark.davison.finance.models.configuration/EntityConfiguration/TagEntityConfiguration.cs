namespace mark.davison.finance.models.configuration.EntityConfiguration;

public class TagEntityConfiguration : FinanceEntityConfiguration<Tag>
{
    public override void ConfigureEntity(EntityTypeBuilder<Tag> builder)
    {
        builder
            .Property(_ => _.Name)
            .HasMaxLength(64);

        ConfigureConversion(builder
            .Property(_ => _.MinDate));

        ConfigureConversion(builder
            .Property(_ => _.MaxDate));
    }
}
