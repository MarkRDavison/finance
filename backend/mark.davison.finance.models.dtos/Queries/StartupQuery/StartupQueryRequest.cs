namespace mark.davison.finance.models.dtos.Queries.StartupQuery;

[GetRequest(Path = "startup-query")]
public class StartupQueryRequest : ICommand<StartupQueryRequest, StartupQueryResponse>
{
}

