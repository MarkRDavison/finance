namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Common;

public class EditTransactionFormSubmission : IFormSubmission<EditTransactionFormViewModel>
{
    private readonly IClientHttpRepository _repository;
    private readonly ICQRSDispatcher _dispatcher;

    public EditTransactionFormSubmission(
        IClientHttpRepository repository,
        ICQRSDispatcher dispatcher
    )
    {
        _repository = repository;
        _dispatcher = dispatcher;
    }

    public async Task<bool> Primary(EditTransactionFormViewModel formViewModel)
    {
        if (!formViewModel.Valid)
        {
            return false;
        }

        var request = new CreateTransactionRequest
        {
            Description = formViewModel.SplitDescription,
            TransactionTypeId = formViewModel.TransactionTypeId,
            Transactions = formViewModel.Items.Select(_ => ToCreateTransactionDto(_, formViewModel)).ToList()
        };

        var response = await _repository.Post<CreateTransactionResponse, CreateTransactionRequest>(request, CancellationToken.None);

        if (response.Success)
        {
            var action = new UpdateTransactionStateItemsAction(response.Transactions);
            await _dispatcher.Dispatch<UpdateTransactionStateItemsAction>(action, CancellationToken.None);

            // TODO: Trigger current balance flags dirty for accounts in transaction


            if (formViewModel.Id == Guid.Empty)
            {
                formViewModel.Id = response.Group.Id;
            }

            return true;
        }

        return false;
    }

    private CreateTransactionDto ToCreateTransactionDto(EditTransactionFormViewModelItem item, EditTransactionFormViewModel formViewModel)
    {
        var account = formViewModel.AccountState.Instance.Accounts.FirstOrDefault(_ => _.Id == item.SourceAccountId);

        return new CreateTransactionDto
        {
            Id = item.Id,
            Description = item.Description,
            SourceAccountId = item.SourceAccountId!.Value,
            DestinationAccountId = item.DestinationAccountId!.Value,
            Date = DateOnly.FromDateTime(formViewModel.Date!.Value),
            Amount = CurrencyRules.ToPersisted(item.Amount!.Value),
            CurrencyId = account?.CurrencyId ?? Guid.Empty,
            ForeignAmount = item.ForeignAmount == null ? null : CurrencyRules.ToPersisted(item.ForeignAmount!.Value),
            ForeignCurrencyId = item.ForeignCurrencyId,
            CategoryId = item.CategoryId,
            // TODO: Remaining fields when implemented
        };
    }
}
