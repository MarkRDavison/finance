namespace mark.davison.finance.bff.queries.Scenarios.CreateAccountQuery;

public class CreateAccountQueryCommandHandler : ICommandHandler<CreateAccountQueryRequest, CreateAccountQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public CreateAccountQueryCommandHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<CreateAccountQueryResponse> Handle(CreateAccountQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = new CreateAccountQueryResponse();

        var banks = await _httpRepository.GetEntitiesAsync<Bank>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        var accountTypes = await _httpRepository.GetEntitiesAsync<AccountType>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        response.Banks.AddRange(banks.Select(_ => new SearchItemDto
        {
            Id = _.Id,
            PrimaryText = _.Name
        }));

        response.AccountTypes.AddRange(accountTypes.Select(_ => new SearchItemDto
        {
            Id = _.Id,
            PrimaryText = _.Type
        }));

        return response;
    }
}

