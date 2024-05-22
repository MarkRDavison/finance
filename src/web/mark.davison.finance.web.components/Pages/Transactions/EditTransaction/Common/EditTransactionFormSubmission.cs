namespace mark.davison.finance.web.components.Pages.Transactions.EditTransaction.Common;

public class EditTransactionFormSubmission : IFormSubmission<EditTransactionFormViewModel>
{
    private readonly IStoreHelper _storeHelper;
    private readonly IDispatcher _dispatcher;
    private readonly IState<AccountState> _accountState;
    private readonly IActionSubscriber _actionSubscriber;

    public EditTransactionFormSubmission(
        IStoreHelper storeHelper,
        IState<AccountState> accountState
,
        IActionSubscriber actionSubscriber,
        IDispatcher dispatcher)
    {
        _storeHelper = storeHelper;
        _accountState = accountState;
        _actionSubscriber = actionSubscriber;
        _dispatcher = dispatcher;
    }

    public async Task<Response> Primary(EditTransactionFormViewModel formViewModel)
    {
        var request = new CreateTransactionRequest
        {
            Description = formViewModel.SplitDescription,
            TransactionTypeId = formViewModel.TransactionTypeId,
            Transactions = formViewModel.Items.Select(_ => ToCreateTransactionDto(_, formViewModel)).ToList()
        };


        var action = new CreateTransactionAction
        {
            ActionId = Guid.NewGuid(),
            Request = request
        };

        if (false)
        {
            TaskCompletionSource tcs = new();
            CreateTransactionActionResponse? result = null;

            _actionSubscriber.SubscribeToAction(
                    this,
                    (CreateTransactionActionResponse actionResponse) =>
                    {
                        if (actionResponse.ActionId == action.ActionId)
                        {
                            result = actionResponse;
                            tcs.SetResult();
                        }
                    });

            using (_actionSubscriber.GetActionUnsubscriberAsIDisposable(this))
            {
                _dispatcher.Dispatch(action);

                await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(50)));
            }

            Console.WriteLine("CreateTransactionActionResponse submitted in submission");
            if (result?.Success ?? false)
            {
                // TODO: Trigger current balance flags dirty for accounts in transaction

                if (formViewModel.Id == Guid.Empty)
                {
                    formViewModel.Id = result.Group.Id;
                }
            }

            return result ?? new() { Errors = ["TODO"] };
        }
        else
        {
            var result = await _storeHelper.DispatchAndWaitForResponse<
                CreateTransactionAction,
                CreateTransactionActionResponse>(action);

            if (result?.Success ?? false)
            {
                // TODO: Trigger current balance flags dirty for accounts in transaction

                if (formViewModel.Id == Guid.Empty)
                {
                    formViewModel.Id = result.Group.Id;
                }
            }


            return result ?? new() { Errors = ["TODO"] };
        }
    }

    private CreateTransactionDto ToCreateTransactionDto(EditTransactionFormViewModelItem item, EditTransactionFormViewModel formViewModel)
    {
        var account = _accountState.Value.Accounts.FirstOrDefault(_ => _.Id == item.SourceAccountId);

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
