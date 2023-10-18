using mark.davison.finance.bff;

namespace mark.davison.finance.api.test;

[mark.davison.common.source.generators.CQRS.UseCQRSServer(
    typeof(BffRootType),
    typeof(CommandsRootType),
    typeof(QueriesRootType),
    typeof(DtosRootType))]
public class ApiTestRoot
{
    public void Test(IEndpointRouteBuilder builder)
    {
    }
}
