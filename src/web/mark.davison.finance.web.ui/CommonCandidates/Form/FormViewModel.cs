namespace mark.davison.finance.web.ui.CommonCandidates.Form;

public abstract class FormViewModel<TFormModel> : IFormViewModel<TFormModel>
    where TFormModel : IFormModel
{

    public EditContext EditContext { get; private set; } = default!;

    public TFormModel Model { get; private set; } = default!;

    public async Task InitialiseAsync()
    {
        Model = await InitialiseModel();
        EditContext = new EditContext(Model);
        EditContext.OnFieldChanged += EditContext_OnFieldChanged;
    }

    private void EditContext_OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        ModelChanged(e);
    }

    protected virtual void ModelChanged(FieldChangedEventArgs e)
    {

    }

    public abstract Task<TFormModel> InitialiseModel();
    public virtual Task Submit() => Task.CompletedTask;
}
