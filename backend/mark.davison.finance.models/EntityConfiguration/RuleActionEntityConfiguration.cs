namespace mark.davison.finance.models.EntityConfiguration;

public partial class RuleActionEntityConfiguration : FinanceEntityConfiguration<RuleAction>
{
    public override void ConfigureEntity(EntityTypeBuilder<RuleAction> builder)
    {
        builder
            .Property(_ => _.ActionType)
            .HasMaxLength(50);

        builder
            .Property(_ => _.ActionValue)
            .HasMaxLength(50);

        builder
            .Property(_ => _.Order);

        builder
            .Property(_ => _.IsActive);

        builder
            .Property(_ => _.StopProcessing);
    }
}

