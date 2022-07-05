namespace mark.davison.finance.common.client.Authentication;

public interface IAuthenticationContext : INotifyPropertyChanged, INotifyPropertyChanging
{
    Guid UserId { get; set; }
    bool IsAuthenticated { get; set; }
    bool IsAuthenticating { get; set; }
    UserProfile? User { get; set; }

    Task ValidateAuthState();
    Task Login();
    Task Logout();

}
