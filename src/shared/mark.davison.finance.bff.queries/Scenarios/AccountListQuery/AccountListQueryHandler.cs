namespace mark.davison.finance.bff.queries.Scenarios.AccountListQuery;

public class AccountListQueryHandler : IQueryHandler<AccountListQueryRequest, AccountListQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public AccountListQueryHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<AccountListQueryResponse> Handle(AccountListQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new AccountListQueryResponse();

        var accounts = await _httpRepository.GetEntitiesAsync<AccountSummary>(
            "account/summary", // TODO: magic string
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellationToken);

        response.Accounts.AddRange(accounts
            .Where(_ =>
                _.Id != Account.OpeningBalance &&
                _.Id != Account.Reconciliation)
            .Select(_ => new AccountListItemDto
            {
                Id = _.Id,
                AccountNumber = _.AccountNumber,
                Name = _.Name,
                AccountType = _.AccountType,
                AccountTypeId = _.AccountTypeId,
                Active = _.IsActive,
                CurrencyId = _.CurrencyId,
                LastModified = _.LastActivity,
                VirtualBalance = _.VirtualBalance,
                OpeningBalance = _.OpeningBalance,
                OpeningBalanceDate = _.OpeningBalanceDate
            }));

        return response;
    }
}

