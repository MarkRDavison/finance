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

        builder.HasData(
            new LinkType { Id = LinkType.Related, Name = "Related", Inward = "relates to", Outward = "relates to", Editable = false, UserId = Guid.Empty },
            new LinkType { Id = LinkType.Refund, Name = "Refund", Inward = "is (partially) refunded by", Outward = "(partially) refunds", Editable = false, UserId = Guid.Empty },
            new LinkType { Id = LinkType.Paid, Name = "Paid", Inward = "is (partially) paid for by", Outward = "(partially) pays for", Editable = false, UserId = Guid.Empty },
            new LinkType { Id = LinkType.Reimbursement, Name = "Reimbursement", Inward = "is (partially) reimbursed by", Outward = "(partially) reimburses", Editable = false, UserId = Guid.Empty }
        );
    }
}

