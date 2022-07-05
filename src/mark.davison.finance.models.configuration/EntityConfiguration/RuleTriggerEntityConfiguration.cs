namespace mark.davison.finance.models.EntityConfiguration;

public partial class RuleTriggerEntityConfiguration : FinanceEntityConfiguration<RuleTrigger>
{
    public override void ConfigureEntity(EntityTypeBuilder<RuleTrigger> builder)
    {
        builder
            .Property(_ => _.TriggerType)
            .HasMaxLength(50);

        builder
            .Property(_ => _.TriggerValue)
            .HasMaxLength(50);

        builder
            .Property(_ => _.Order);

        builder
            .Property(_ => _.IsActive);

        builder
            .Property(_ => _.StopProcessing);
    }
}

