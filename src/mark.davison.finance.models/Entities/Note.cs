namespace mark.davison.finance.models.Entities;

public partial class Note : FinanceEntity
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string NoteableType { get; set; } = string.Empty;
    public int NoteableId { get; set; } // TODO: What is this?
}

