namespace mark.davison.finance.models.EntityConfiguration;

public partial class JournalLinkEntityConfiguration : FinanceEntityConfiguration<JournalLink>
{
    public override void ConfigureEntity(EntityTypeBuilder<JournalLink> builder)
    {
        builder
            .Property(_ => _.Comment)
            .HasMaxLength(1024);
    }
}

