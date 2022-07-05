namespace mark.davison.finance.models.EntityConfiguration;

public partial class RuleGroupEntityConfiguration : FinanceEntityConfiguration<RuleGroup>
{
    public override void ConfigureEntity(EntityTypeBuilder<RuleGroup> builder)
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
    }
}

