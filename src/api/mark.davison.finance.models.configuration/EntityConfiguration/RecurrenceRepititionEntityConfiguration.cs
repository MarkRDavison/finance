namespace mark.davison.finance.models.EntityConfiguration;

public partial class RecurrenceRepititionEntityConfiguration : FinanceEntityConfiguration<RecurrenceRepitition>
{
    public override void ConfigureEntity(EntityTypeBuilder<RecurrenceRepitition> builder)
    {
        builder
            .Property(_ => _.RepititionType)
            .HasMaxLength(50);

        builder
            .Property(_ => _.RepititionMoment)
            .HasMaxLength(50);

        builder
            .Property(_ => _.RepititionSkip);

        builder
            .Property(_ => _.Weekend);
    }
}

