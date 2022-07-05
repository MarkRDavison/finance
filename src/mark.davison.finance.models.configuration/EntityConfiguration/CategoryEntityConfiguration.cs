namespace mark.davison.finance.models.EntityConfiguration;

public partial class CategoryEntityConfiguration : FinanceEntityConfiguration<Category>
{
    public override void ConfigureEntity(EntityTypeBuilder<Category> builder)
    {
        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);
    }
}

