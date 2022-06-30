namespace mark.davison.finance.models.EntityConfiguration;

public partial class BudgetEntityConfiguration : FinanceEntityConfiguration<Budget>
{
    public override void ConfigureEntity(EntityTypeBuilder<Budget> builder)
    {
        builder
            .Property(_ => _.IsActive);

        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.Order);
    }
}

