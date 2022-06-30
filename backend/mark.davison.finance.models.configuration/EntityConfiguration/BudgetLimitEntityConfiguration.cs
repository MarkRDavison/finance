namespace mark.davison.finance.models.EntityConfiguration;

public partial class BudgetLimitEntityConfiguration : FinanceEntityConfiguration<BudgetLimit>
{
    public override void ConfigureEntity(EntityTypeBuilder<BudgetLimit> builder)
    {
        ConfigureConversion(builder
            .Property(_ => _.StartDate));

        ConfigureConversion(builder
            .Property(_ => _.EndDate));

        builder
            .Property(_ => _.Amount);

        builder
            .Property(_ => _.Period)
            .HasMaxLength(PeriodMaxLength);

        builder
            .Property(_ => _.Generated);
    }
}

