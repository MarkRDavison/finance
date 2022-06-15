namespace mark.davison.finance.bff.queries.Scenarios.CreateAccountQuery;

public class CreateAccountQueryResponse
{
    public List<SearchItemDto> Banks { get; set; } = new();
    public List<SearchItemDto> AccountTypes { get; set; } = new();
}

