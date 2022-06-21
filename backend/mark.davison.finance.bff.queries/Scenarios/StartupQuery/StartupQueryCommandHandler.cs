namespace mark.davison.finance.bff.queries.Scenarios.StartupQuery;

public class StartupQueryCommandHandler : ICommandHandler<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IHttpRepository _httpRepository;

    public StartupQueryCommandHandler(IHttpRepository httpRepository)
    {
        _httpRepository = httpRepository;
    }

    public async Task<StartupQueryResponse> Handle(StartupQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellation)
    {
        var response = new StartupQueryResponse();

        var banks = await _httpRepository.GetEntitiesAsync<Bank>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        var accountTypes = await _httpRepository.GetEntitiesAsync<AccountType>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        var currencies = await _httpRepository.GetEntitiesAsync<Currency>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        response.Banks.AddRange(banks.Select(_ => new BankDto
        {
            Id = _.Id,
            Name = _.Name
        }));

        response.AccountTypes.AddRange(accountTypes.Select(_ => new AccountTypeDto
        {
            Id = _.Id,
            Type = _.Type
        }));

        response.Currencies.AddRange(currencies.Select(_ =>
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

