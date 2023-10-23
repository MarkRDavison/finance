namespace mark.davison.finance.accounting.rules.Account;

public static class BuiltinAccountNames
{
    public static Guid OpeningBalance = new Guid("0D88BF03-BA3C-4083-9955-80BAF27CB657");
    public static Guid Reconciliation = new Guid("F1B34475-29C9-4379-A26B-2197230A14FD");

    public static string GetBuiltinAccountName(Guid id)
    {
        if (id == OpeningBalance)
        {
            return "Opening balance";
        }
        else if (id == Reconciliation)
        {
            return "Reconciliation";
        }
        return string.Empty;
    }
}
