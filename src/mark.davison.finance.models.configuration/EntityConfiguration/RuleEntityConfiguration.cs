namespace mark.davison.finance.models.EntityConfiguration;

public partial class RuleEntityConfiguration : FinanceEntityConfiguration<Rule>
{
    public override void ConfigureEntity(EntityTypeBuilder<Rule> builder)
    {
        builder
            .Property(_ => _.Title)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.Description)
            .HasMaxLength(1024);

        builder
            .Property(_ => _.Order);

        builder
            .Property(_ => _.IsActive);

        builder
            .Property(_ => _.StopProcessing);

        builder
            .Property(_ => _.Strict);
    }
}

