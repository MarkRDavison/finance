namespace mark.davison.finance.web.ui.Pages.Categories.AddCategory;

public class AddCategoryModalViewModal
{
    private readonly ICQRSDispatcher _dispatcher;

    public AddCategoryModalViewModal(
        ICQRSDispatcher dispatcher
    )
    {
        _dispatcher = dispatcher;
    }

    public AddCategoryFormViewModel AddCategoryFormViewModel { get; set; } = new();

    public async Task<bool> OnSave()
    {
        if (!AddCategoryFormViewModel.Valid)
        {
            return false;
        }

        var response = await _dispatcher.Dispatch<CreateCategoryListCommand, CreateCategoryListCommandResponse>(new CreateCategoryListCommand
        {
            Name = AddCategoryFormViewModel.Name
        }, CancellationToken.None);

        if (response.Success && response.ItemId != Guid.Empty)
        {
            await _dispatcher.Dispatch(new UpdateCategoryListAction(new List<CategoryListItemDto> {
                new CategoryListItemDto{Id = response.ItemId, Name = AddCategoryFormViewModel.Name }
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
