namespace mark.davison.finance.bff.queries.Scenarios.StartupQuery;

[GetRequest(Path = "startup-query")]
public class StartupQueryRequest : ICommand<StartupQueryRequest, StartupQueryResponse>
{
}

