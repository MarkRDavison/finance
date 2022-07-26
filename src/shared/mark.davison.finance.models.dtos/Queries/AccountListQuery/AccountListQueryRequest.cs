namespace mark.davison.finance.models.dtos.Queries.AccountListQuery;

[GetRequest(Path = "account-list-query")]
public class AccountListQueryRequest : IQuery<AccountListQueryRequest, AccountListQueryResponse>
{
    public bool ShowActive { get; set; } = false;
}

