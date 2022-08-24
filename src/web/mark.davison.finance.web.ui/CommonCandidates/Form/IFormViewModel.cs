namespace mark.davison.finance.web.ui.CommonCandidates.Form;

public interface IFormViewModel<TFormModel>
    where TFormModel : IFormModel
{
    public EditContext EditContext { get; }
    public TFormModel Model { get; }

    Task Submit();
    Task InitialiseAsync();
    Task<TFormModel> InitialiseModel();
}
