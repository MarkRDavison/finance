namespace mark.davison.finance.bff.queries.Scenarios.CreateAccountQuery;

[GetRequest(Path = "create-account-query")]
public class CreateAccountQueryRequest : ICommand<CreateAccountQueryRequest, CreateAccountQueryResponse>
{
}

