namespace mark.davison.finance.models.EntityConfiguration;

public partial class RecurrenceEntityConfiguration : FinanceEntityConfiguration<Recurrence>
{
    public override void ConfigureEntity(EntityTypeBuilder<Recurrence> builder)
    {
        builder
            .Property(_ => _.Title)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.Description)
            .HasMaxLength(1024);

        ConfigureConversion(builder
            .Property(_ => _.FirstDate));

        ConfigureConversion(builder
            .Property(_ => _.RepeatUntil));

        ConfigureConversion(builder
            .Property(_ => _.LastDate));

        builder
            .Property(_ => _.Repetitions);

        builder
            .Property(_ => _.ApplyRules);

        builder
            .Property(_ => _.IsActive);
    }
}

