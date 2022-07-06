namespace mark.davison.finance.models.EntityConfiguration;

public partial class TransactionEntityConfiguration : FinanceEntityConfiguration<Transaction>
{
    public override void ConfigureEntity(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(_ => _.Description)
            .HasMaxLength(1024);

        builder
            .Property(_ => _.Amount);

        builder
            .Property(_ => _.ForeignAmount);

        builder
            .Property(_ => _.Reconciled);
    }
}

