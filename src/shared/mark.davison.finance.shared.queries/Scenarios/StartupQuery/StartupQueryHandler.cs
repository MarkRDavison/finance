namespace mark.davison.finance.shared.queries.Scenarios.StartupQuery;

public class StartupQueryHandler : IQueryHandler<StartupQueryRequest, StartupQueryResponse>
{
    private readonly IFinanceDbContext _dbContext;
    private readonly IFinanceUserContext _financeUserContext;

    public StartupQueryHandler(
        IFinanceDbContext dbContext,
        IFinanceUserContext financeUserContext)
    {
        _dbContext = dbContext;
        _financeUserContext = financeUserContext;
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
            UserContext = new()
            {
                StartRange = _financeUserContext.RangeStart,
                EndRange = _financeUserContext.RangeEnd
            },
            AccountTypes = accountTypes,
            TransactionTypes = transactionTypes,
            Currencies = currencies
        };

        return response;
    }
}

