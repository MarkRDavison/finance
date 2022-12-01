namespace mark.davison.finance.web.components;

public class Routes
{
    public const string Root = "/";

    public const string Accounts = "/accounts";
    public const string Account = "/accounts/{id:guid}";

    public const string Categories = "/categories";
    public const string Category = "/categories/{id:guid}";

    public const string Tags = "/tags";
    public const string Tag = "/tags/{id:guid}";

    public const string Transactions = "/transactions";
    public const string Transaction = "/transactions/{id:guid}";
    public const string TransactionNew = "/transactions/new/{type:guid}";
}

public static class RouteHelpers
{
    public static string Account(Guid id) => Routes.Account.Replace("{id:guid}", id.ToString());
    public static string Category(Guid id) => Routes.Category.Replace("{id:guid}", id.ToString());
    public static string Tag(Guid id) => Routes.Tag.Replace("{id:guid}", id.ToString());
    public static string TransactionNew(Guid type) => Routes.TransactionNew.Replace("{type:guid}", type.ToString());
    public static string Transaction(Guid id) => Routes.Transaction.Replace("{id:guid}", id.ToString());
}