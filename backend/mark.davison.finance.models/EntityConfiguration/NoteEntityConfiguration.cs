namespace mark.davison.finance.models.EntityConfiguration;

public partial class NoteEntityConfiguration : FinanceEntityConfiguration<Note>
{
    public override void ConfigureEntity(EntityTypeBuilder<Note> builder)
    {
        builder
            .Property(_ => _.Title)
            .HasMaxLength(NameMaxLength);

        builder
            .Property(_ => _.Text)
            .HasMaxLength(1024);

        builder
            .Property(_ => _.NoteableType)
            .HasMaxLength(50);

        builder
            .Property(_ => _.NoteableId);
    }
}

