namespace mark.davison.finance.common.client.Authentication;

public class AuthenticationConfig : IAuthenticationConfig
{
    public string LoginEndpoint { get; set; } = string.Empty;
    public string LogoutEndpoint { get; set; } = string.Empty;
    public string UserEndpoint { get; set; } = string.Empty;
    public void SetBffBase(string bffBase)
    {
        LoginEndpoint = bffBase + "/auth/login";
        LogoutEndpoint = bffBase + "/auth/logout";
        UserEndpoint = bffBase + "/auth/user";
    }
}
