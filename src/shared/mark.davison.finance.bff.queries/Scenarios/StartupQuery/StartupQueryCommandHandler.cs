namespace mark.davison.finance.bff.queries.Scenarios.StartupQuery;

public class StartupQueryCommandHandler : IQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IRepository _repository;

    public StartupQueryCommandHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<StartupQueryResponse> Handle(StartupQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new StartupQueryResponse();

        await using (_repository.BeginTransaction())
        {
            var accountTypes = await _repository.GetEntitiesAsync<AccountType>(
                cancellationToken);

            var currencies = await _repository.GetEntitiesAsync<Currency>(
                cancellationToken);

            var transactionTypes = await _repository.GetEntitiesAsync<TransactionType>(
                cancellationToken);

            response.AccountTypes.AddRange(accountTypes.Select(_ =>
            new AccountTypeDto
            {
                Id = _.Id,
                Type = _.Type
            }));

            response.TransactionTypes.AddRange(transactionTypes.Select(_ =>
            new TransactionTypeDto
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
        }

        return response;
    }
}

