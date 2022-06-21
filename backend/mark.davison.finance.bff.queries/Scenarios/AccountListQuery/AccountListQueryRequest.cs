namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

[GetRequest(Path = "account-list-query")]
public class AccountListQueryRequest : ICommand<AccountListQueryRequest, AccountListQueryResponse>
{
    public bool ShowActive { get; set; } = false;
}

