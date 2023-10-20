namespace mark.davison.finance.web.components.CommonCandidates.Form;

public class Form<TFormViewModel> : ComponentBase where TFormViewModel : IFormViewModel
{
    [Parameter, EditorRequired]
    public required TFormViewModel FormViewModel { get; set; }
}
