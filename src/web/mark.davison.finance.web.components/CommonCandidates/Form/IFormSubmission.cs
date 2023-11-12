namespace mark.davison.finance.web.components.CommonCandidates.Form;

public interface IFormSubmission<TFormViewModel> where TFormViewModel : IFormViewModel
{
    Task<bool> Primary(TFormViewModel formViewModel);
}
