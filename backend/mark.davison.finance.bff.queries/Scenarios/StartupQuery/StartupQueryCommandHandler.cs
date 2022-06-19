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

        var currencies = await _httpRepository.GetEntitiesAsync<Currency>(
            new QueryParameters(),
            HeaderParameters.Auth(currentUserContext.Token, currentUserContext.CurrentUser),
            cancellation);

        response.Currencies.AddRange(currencies.Select(_ =>
        new CurrencyDto
        {
            Code = _.Code,
            Name = _.Name,
            DecimalPlaces = _.DecimalPlaces,
            Symbol = _.Symbol
        }));

        return response;
    }
}

