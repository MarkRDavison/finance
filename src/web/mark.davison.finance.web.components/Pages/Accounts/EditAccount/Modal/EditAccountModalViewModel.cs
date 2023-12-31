﻿namespace mark.davison.finance.web.components.Pages.Accounts.EditAccount.Modal;

// TODO: Extract all the logic for on save to a helper, so we can have an edit account modal/page etc
public class EditAccountModalViewModel : IModalViewModel<EditAccountFormViewModel, EditAccountForm>
{
    private readonly IFormSubmission<EditAccountFormViewModel> _editAccountFormSubmission;

    public EditAccountModalViewModel(
        IFormSubmission<EditAccountFormViewModel> editAccountFormSubmission
    )
    {
        _editAccountFormSubmission = editAccountFormSubmission;
    }

    public EditAccountFormViewModel FormViewModel { get; set; } = new();

    public Task<bool> Primary(EditAccountFormViewModel formViewModel) => _editAccountFormSubmission.Primary(formViewModel);
}
