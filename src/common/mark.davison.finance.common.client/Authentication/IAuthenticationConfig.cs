namespace mark.davison.finance.common.client.Authentication;

public interface IAuthenticationConfig
{
    string LoginEndpoint { get; set; }
    string LogoutEndpoint { get; set; }
    string UserEndpoint { get; set; }
    void SetBffBase(string bffBase);
}
