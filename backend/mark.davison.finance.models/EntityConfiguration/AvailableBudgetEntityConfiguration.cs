namespace mark.davison.finance.models.EntityConfiguration;

public partial class AvailableBudgetEntityConfiguration : FinanceEntityConfiguration<AvailableBudget>
{
    public override void ConfigureEntity(EntityTypeBuilder<AvailableBudget> builder)
    {
        builder
            .Property(_ => _.Amount);

        ConfigureConversion(builder
            .Property(_ => _.StartDate));

        ConfigureConversion(builder
            .Property(_ => _.EndDate));
    }
}

