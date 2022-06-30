using mark.davison.finance.models.dtos.Queries.AccountListQuery;

namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

public class AccountListQueryResponse
{
    public List<AccountListItemDto> Accounts { get; set; } = new();
}

