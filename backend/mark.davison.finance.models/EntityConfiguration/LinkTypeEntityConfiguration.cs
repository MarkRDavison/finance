namespace mark.davison.finance.models.EntityConfiguration;

public partial class LinkTypeEntityConfiguration : FinanceEntityConfiguration<LinkType>
{
    public override void ConfigureEntity(EntityTypeBuilder<LinkType> builder)
    {
        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.Outward)
            .HasMaxLength(255);

        builder
            .Property(_ => _.Inward)
            .HasMaxLength(255);

        builder
            .Property(_ => _.Editable);
    }
}

