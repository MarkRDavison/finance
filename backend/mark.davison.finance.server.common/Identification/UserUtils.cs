namespace mark.davison.finance.common.server.Identification;

public static class ClaimsPrincipalExtensions
{
    public static string SubjectId(this ClaimsPrincipal user)
    {
        return user.Claims
            .FirstOrDefault(c =>
                c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase) ||
                c.Type.Equals("sub", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
    }
    public static Guid ProxiedUserSubjectId(this ClaimsPrincipal user)
    {
        var id = user.Claims?
            .FirstOrDefault(c =>
                c.Type.Equals("ProxiedUserId", StringComparison.OrdinalIgnoreCase))?.Value
            ?? user.SubjectId();

        Guid userId;
        if (Guid.TryParse(id, out userId))
        {
            return userId;
        }

        return Guid.Empty;
    }
}

public class UserUtils
{
    public static UserProfile Create(ClaimsPrincipal user)
    {
        return new UserProfile
        {
            sub = user.ProxiedUserSubjectId()
        };
    }
}
