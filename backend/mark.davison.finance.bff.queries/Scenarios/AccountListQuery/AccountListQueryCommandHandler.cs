namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

public class AccountListQueryCommandHandler : ICommandHandler<AccountListQueryRequest, AccountListQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public AccountListQueryCommandHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<AccountListQueryResponse> Handle(AccountListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = new AccountListQueryResponse();

        var accounts = await _httpRepository.GetEntitiesAsync<AccountSummary>(
            "account/summary",
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        response.Accounts.AddRange(accounts.Select(_ => new AccountListItemDto
        {
            Id = _.Id,
            AccountNumber = _.AccountNumber,
            Name = _.Name,
            AccountType = _.AccountType,
            Active = _.IsActive,
            LastModified = _.LastActivity
        }));

        return response;
    }
}

