namespace mark.davison.finance.web.components.CommonCandidates.Form.Example;

public class ExampleModalViewModel : IModalViewModel<ExampleFormViewModel, ExampleForm>
{
    public ExampleFormViewModel FormViewModel { get; set; } = new();

    public async Task<bool> Primary(ExampleFormViewModel formViewModel)
    {
        await Task.Delay(1500);
        return false;
    }
}
