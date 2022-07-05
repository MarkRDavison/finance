namespace mark.davison.finance.models.EntityConfiguration;

public partial class TransactionTypeEntityConfiguration : FinanceEntityConfiguration<TransactionType>
{
    public override void ConfigureEntity(EntityTypeBuilder<TransactionType> builder)
    {
        builder
            .Property(_ => _.Type)
            .HasMaxLength(50);
    }
}

