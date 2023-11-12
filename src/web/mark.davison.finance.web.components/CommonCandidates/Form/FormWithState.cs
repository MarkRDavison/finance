using mark.davison.common.client.State;

namespace mark.davison.finance.web.components.CommonCandidates.Form;

public class FormWithState<TFormViewModel> : ComponentWithState, IForm<TFormViewModel> where TFormViewModel : IFormViewModel
{
    [Parameter, EditorRequired]
    public required TFormViewModel FormViewModel { get; set; }
}
