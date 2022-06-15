using static mark.davison.finance.common.server.Authentication.ZenoAuthenticationConstants;

namespace mark.davison.finance.bff.test.Authentication;

[TestClass]
public class AuthenticationTests : IntegrationTestBase
{
    [TestMethod]
    public async Task LoginCallback_CreatesUser_WhereUserDoesNotExist()
    {
        await GetAsync(ZenoRouteNames.LoginCallbackRoute);
    }
}
