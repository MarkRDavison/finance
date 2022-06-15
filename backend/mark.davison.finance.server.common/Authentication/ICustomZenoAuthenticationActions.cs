namespace mark.davison.finance.common.server.Authentication;

public interface ICustomZenoAuthenticationActions
{
    Task OnUserAuthenticated(UserProfile userProfile, CancellationToken cancellationToken);
}
