namespace mark.davison.finance.accounting.rules.Account;

public static class BuiltinAccountNames
{
    public static string GetBuiltinAccountName(Guid id)
    {
        if (id == AccountConstants.OpeningBalance)
        {
            return "Opening balance";
        }
        else if (id == AccountConstants.Reconciliation)
        {
            return "Reconciliation";
        }
        return string.Empty;
    }
}
