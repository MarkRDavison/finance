namespace mark.davison.finance.models.EntityConfiguration;

public abstract class FinanceEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : FinanceEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .ValueGeneratedNever();

        builder
            .Property(_ => _.Created);
        builder
            .Property(_ => _.LastModified);

        builder
            .HasOne(_ => _.User)
            .WithMany()
            .HasForeignKey(_ => _.UserId);

        NavigationPropertyEntityConfigurations.ConfigureEntity(builder);
        ConfigureEntity(builder);
    }

    public const int NameMaxLength = 255;
    public const int PeriodMaxLength = 63;


    protected void ConfigureConversion(PropertyBuilder<DateOnly> propertyBuilder)
    {
        propertyBuilder
            .HasConversion(
                _ => _.ToDateTime(TimeOnly.MinValue),
                _ => DateOnly.FromDateTime(_));
    }
    protected void ConfigureConversion(PropertyBuilder<DateOnly?> propertyBuilder)
    {
        propertyBuilder
            .HasConversion(
                _ => _ == null ? null : new DateTime?(_.Value.ToDateTime(TimeOnly.MinValue)),
                _ => _ == null ? null : new DateOnly?(DateOnly.FromDateTime(_.Value)));
    }

    public abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}

