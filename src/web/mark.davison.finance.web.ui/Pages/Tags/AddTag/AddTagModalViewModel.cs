namespace mark.davison.finance.web.ui.Pages.Tags.AddTag;

public class AddTagModalViewModel
{
    private readonly ICQRSDispatcher _dispatcher;

    public AddTagModalViewModel(
        ICQRSDispatcher dispatcher
    )
    {
        _dispatcher = dispatcher;
    }

    public AddTagFormViewModel AddTagFormViewModel { get; set; } = new();

    public async Task<bool> OnSave()
    {
        if (!AddTagFormViewModel.Valid)
        {
            return false;
        }

        var response = await _dispatcher.Dispatch<CreateTagListCommandRequest, CreateTagListCommandResponse>(new CreateTagListCommandRequest
        {
            Name = AddTagFormViewModel.Name
        }, CancellationToken.None);

        if (response.Success && response.ItemId != Guid.Empty)
        {
            await _dispatcher.Dispatch(new UpdateTagListAction(new List<TagDto>
            {
                new TagDto
                {
                    Id = response.ItemId,
                    Name = AddTagFormViewModel.Name
                }
            }), CancellationToken.None);

            return true;
        }

        return false;
    }

    public async Task OnCancel()
    {
        await Task.CompletedTask;
    }

}
