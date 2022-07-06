namespace mark.davison.finance.models.EntityConfiguration;

public partial class TransactionJournalEntityConfiguration : FinanceEntityConfiguration<TransactionJournal>
{
    public override void ConfigureEntity(EntityTypeBuilder<TransactionJournal> builder)
    {
        builder
            .Property(_ => _.Description)
            .HasMaxLength(1024);

        ConfigureConversion(builder
            .Property(_ => _.Date));

        ConfigureConversion(builder
            .Property(_ => _.InterestDate));

        ConfigureConversion(builder
            .Property(_ => _.ProcessDate));

        builder
            .Property(_ => _.Order);

        builder
            .Property(_ => _.Completed);
    }
}

