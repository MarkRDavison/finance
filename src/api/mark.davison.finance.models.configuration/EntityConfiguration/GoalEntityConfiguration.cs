namespace mark.davison.finance.models.EntityConfiguration;

public partial class GoalEntityConfiguration : FinanceEntityConfiguration<Goal>
{
    public override void ConfigureEntity(EntityTypeBuilder<Goal> builder)
    {
        builder
            .Property(_ => _.IsActive);

        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.TargetAmount);

        ConfigureConversion(builder
            .Property(_ => _.StartDate));

        ConfigureConversion(builder
            .Property(_ => _.TargetDate));
    }
}

