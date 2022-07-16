namespace mark.davison.finance.web.ui.Shared;

public class Routes
{
    public const string Root = "/";
    public const string Accounts = "/Accounts";
    public const string Account = "/Accounts/{id:guid}";
}

public static class RouteHelpers
{
    public static string Account(Guid id) => Routes.Account.Replace("{id:guid}", id.ToString());
}