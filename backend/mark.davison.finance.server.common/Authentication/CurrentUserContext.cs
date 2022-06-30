using mark.davison.finance.common.server.abstractions.Authentication;
using mark.davison.finance.common.server.abstractions.Identification;

namespace mark.davison.finance.common.server.Authentication;

public class CurrentUserContext : ICurrentUserContext
{
    public User CurrentUser { get; set; } = null!;
    public string Token { get; set; } = null!;
}

