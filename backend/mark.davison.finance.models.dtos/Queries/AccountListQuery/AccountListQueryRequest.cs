namespace mark.davison.finance.models.dtos.Queries.AccountListQuery;

[GetRequest(Path = "account-list-query")]
public class AccountListQueryRequest : ICommand<AccountListQueryRequest, AccountListQueryResponse>
{
    public bool ShowActive { get; set; } = false;
}

