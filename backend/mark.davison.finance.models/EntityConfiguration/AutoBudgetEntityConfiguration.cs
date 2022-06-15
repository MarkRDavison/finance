namespace mark.davison.finance.models.EntityConfiguration;

public partial class AutoBudgetEntityConfiguration : FinanceEntityConfiguration<AutoBudget>
{
    public override void ConfigureEntity(EntityTypeBuilder<AutoBudget> builder)
    {
        builder
            .Property(_ => _.AutoBudgetType);

        builder
            .Property(_ => _.Amount);

        builder
            .Property(_ => _.Period)
            .HasMaxLength(PeriodMaxLength);
    }
}

