namespace mark.davison.finance.models.EntityConfiguration;

public partial class GoalEventEntityConfiguration : FinanceEntityConfiguration<GoalEvent>
{
    public override void ConfigureEntity(EntityTypeBuilder<GoalEvent> builder)
    {
        ConfigureConversion(builder
            .Property(_ => _.Date));

        builder
            .Property(_ => _.Amount);
    }
}

