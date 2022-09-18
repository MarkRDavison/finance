namespace mark.davison.finance.bff.queries.Scenarios.StartupQuery;

public class StartupQueryCommandHandler : IQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public StartupQueryCommandHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<StartupQueryResponse> Handle(StartupQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new StartupQueryResponse();

        var accountTypes = await _httpRepository.GetEntitiesAsync<AccountType>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellationToken);

        var currencies = await _httpRepository.GetEntitiesAsync<Currency>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellationToken);

        var transactionTypes = await _httpRepository.GetEntitiesAsync<TransactionType>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellationToken);

        response.AccountTypes.AddRange(accountTypes.Select(_ => new AccountTypeDto
        {
            Id = _.Id,
            Type = _.Type
        }));

        response.TransactionTypes.AddRange(transactionTypes.Select(_ => new TransactionTypeDto
        {
            Id = _.Id,
            Type = _.Type
        }));

        response.Currencies.AddRange(currencies.Where(_ => _.Id != Currency.INT).Select(_ =>
        new CurrencyDto
        {
            Id = _.Id,
            Code = _.Code,
            Name = _.Name,
            DecimalPlaces = _.DecimalPlaces,
            Symbol = _.Symbol
        }));

        return response;
    }
}

