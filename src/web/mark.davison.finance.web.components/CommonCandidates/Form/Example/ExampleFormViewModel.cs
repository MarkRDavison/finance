namespace mark.davison.finance.web.components.CommonCandidates.Form.Example;

public class ExampleFormViewModel : IFormViewModel
{
    public string Text { get; set; } = "Existing value";
    public DateTime? Date { get; set; }
    public bool Valid => !string.IsNullOrEmpty(Text) && Date != default;
}
