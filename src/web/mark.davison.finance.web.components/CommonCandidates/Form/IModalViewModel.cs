namespace mark.davison.finance.web.components.CommonCandidates.Form;

public interface IModalViewModel<TFormViewModel, TForm>
    where TFormViewModel : IFormViewModel
    where TForm : ComponentBase
{
    TFormViewModel FormViewModel { get; set; }

    Task<bool> Primary(TFormViewModel formViewModel);
}
