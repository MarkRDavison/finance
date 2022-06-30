using mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

namespace mark.davison.finance.web.ui.Features.AccountList;

public partial class AccountListState : State<AccountListState>
{
    public class FetchAccountListAction : IAction
    {

    }

    public class FetchAccountListActionHandler : ActionHandler<FetchAccountListAction>
    {
        private readonly IClientHttpRepository _repository;

        public FetchAccountListActionHandler(
            IStore store,
            IClientHttpRepository repository
        ) : base(store)
        {
            _repository = repository;
        }

        public override async Task<Unit> Handle(FetchAccountListAction aAction, CancellationToken aCancellationToken)
        {
            var request = new AccountListQueryRequest
            {
                ShowActive = false
            };

            var response = await _repository.Get<AccountListQueryResponse, AccountListQueryRequest>(request);

            Store.SetState(new AccountListState(response.Accounts));

            return Unit.Value;
        }
    }

}
