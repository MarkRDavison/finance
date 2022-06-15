namespace mark.davison.finance.models.EntityConfiguration;

public partial class RecurringTransactionEntityConfiguration : FinanceEntityConfiguration<RecurringTransaction>
{
    public override void ConfigureEntity(EntityTypeBuilder<RecurringTransaction> builder)
    {
        builder
            .Property(_ => _.Amount);

        builder
            .Property(_ => _.ForeignAmount);

        builder
            .Property(_ => _.Description)
            .HasMaxLength(1024);
    }
}

