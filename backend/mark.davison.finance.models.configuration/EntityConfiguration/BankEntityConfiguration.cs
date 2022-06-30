namespace mark.davison.finance.models.EntityConfiguration;

public partial class BankEntityConfiguration : FinanceEntityConfiguration<Bank>
{
    public override void ConfigureEntity(EntityTypeBuilder<Bank> builder)
    {
        builder
            .Property(_ => _.Name)
            .HasMaxLength(NameMaxLength);
    }
}
