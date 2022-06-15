namespace mark.davison.finance.models.EntityConfiguration;

public partial class TransactionGroupEntityConfiguration : FinanceEntityConfiguration<TransactionGroup>
{
    public override void ConfigureEntity(EntityTypeBuilder<TransactionGroup> builder)
    {
        builder
            .Property(_ => _.Title)
            .HasMaxLength(NameMaxLength);
    }
}

