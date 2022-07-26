using mark.davison.common.Identification;
using mark.davison.common.server.abstractions.Authentication;
using mark.davison.common.server.abstractions.Identification;
using mark.davison.common.server.abstractions.Repository;

namespace mark.davison.finance.bff.Authentication;

public class FinanceCustomZenoAuthenticationActions : ICustomZenoAuthenticationActions
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpRepository _httpRepository;

    public FinanceCustomZenoAuthenticationActions(
        IHttpContextAccessor httpContextAccessor,
        IHttpRepository httpRepository
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _httpRepository = httpRepository;
    }

    private Task<User?> GetUser(Guid sub, CancellationToken cancellationToken)
    {
        return _httpRepository.GetEntityAsync<User>(
               new QueryParameters { { nameof(User.Sub), sub.ToString() } },
               HeaderParameters.None,
               cancellationToken);
    }

    private Task<User?> UpsertUser(UserProfile userProfile, string token, CancellationToken cancellationToken)
    {
        return _httpRepository.UpsertEntityAsync(
                new User
                {
                    Id = Guid.NewGuid(),
                    Sub = userProfile.sub,
                    Admin = false,
                    Created = DateTime.UtcNow,
                    Email = userProfile.email!,
                    First = userProfile.given_name!,
                    Last = userProfile.family_name!,
                    LastModified = DateTime.UtcNow,
                    Username = userProfile.preferred_username!
                },
                HeaderParameters.Auth(token, null),
                cancellationToken);
    }

    public async Task OnUserAuthenticated(UserProfile userProfile, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor
            ?.HttpContext
            ?.Session
            ?.GetString(ZenoAuthenticationConstants.SessionNames.AccessToken);
        // TODO: Wrapper service around session access
        var user = await GetUser(userProfile.sub, cancellationToken);

        if (user == null && !string.IsNullOrEmpty(token))
        {
            user = await UpsertUser(userProfile, token, cancellationToken);
        }

        if (user != null)
        {
            _httpContextAccessor!.HttpContext!.Session.SetString(ZenoAuthenticationConstants.SessionNames.User, JsonSerializer.Serialize(user));
        }
    }
}
