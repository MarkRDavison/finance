namespace mark.davison.finance.shared.queries.Scenarios.StartupQuery;

// TODO: Fix name
public class StartupQueryCommandHandler : IQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;

    public StartupQueryCommandHandler(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartupQueryResponse> Handle(StartupQueryRequest query, ICurrentUserContext currentUserContext, CancellationToken cancellationToken)
    {
        var response = new StartupQueryResponse();

        var accountTypes = await _dbContext
            .Set<AccountType>()
            .AsNoTracking()
            .Select(_ => new AccountTypeDto
            {
                Id = _.Id,
                Type = _.Type
            })
            .ToListAsync(cancellationToken);

        var currencies = await _dbContext
            .Set<Currency>()
            .AsNoTracking()
            .Where(_ => _.Id != Currency.INT)
            .Select(_ => new CurrencyDto
            {
                Id = _.Id,
                Code = _.Code,
                Name = _.Name,
                DecimalPlaces = _.DecimalPlaces,
                Symbol = _.Symbol
            })
            .ToListAsync(cancellationToken);

        var transactionTypes = await _dbContext
            .Set<TransactionType>()
            .AsNoTracking()
            .Select(_ => new TransactionTypeDto
            {
                Id = _.Id,
                Type = _.Type
            })
            .ToListAsync(cancellationToken);

        response.Value = new()
        {
            AccountTypes = accountTypes,
            TransactionTypes = transactionTypes,
            Currencies = currencies
        };

        return response;
    }
}

