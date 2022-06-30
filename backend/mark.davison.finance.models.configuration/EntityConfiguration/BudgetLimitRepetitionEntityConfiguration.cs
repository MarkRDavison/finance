namespace mark.davison.finance.models.EntityConfiguration;

public partial class BudgetLimitRepetitionEntityConfiguration : FinanceEntityConfiguration<BudgetLimitRepetition>
{
    public override void ConfigureEntity(EntityTypeBuilder<BudgetLimitRepetition> builder)
    {

        ConfigureConversion(builder
            .Property(_ => _.StartDate));

        ConfigureConversion(builder
            .Property(_ => _.EndDate));

        builder
            .Property(_ => _.Amount);
    }
}

