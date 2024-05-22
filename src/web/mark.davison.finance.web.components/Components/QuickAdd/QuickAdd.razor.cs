namespace mark.davison.finance.web.components.Components.QuickAdd;

public partial class QuickAdd
{
    private class QuickAddItem
    {
        public required string Text { get; init; }
        public required string Href { get; init; }
        public required string Icon { get; init; }
        public required bool FlipX { get; init; }
    }

    private List<QuickAddItem> Items { get; init; }
    private void OnLinkClick(string href)
    {
        OnSelect();
        _navigation.NavigateTo(href);
    }

    [Parameter, EditorRequired]
    public required Action OnSelect { get; set; }

    public QuickAdd()
    {
        // TODO: i8n
        Items = new List<QuickAddItem>
        {
            new()
            {
                Text = "New withdrawal",
                Href = RouteHelpers.TransactionNew(TransactionConstants.Withdrawal),
                Icon = Icons.Material.Filled.ArrowRightAlt,
                FlipX = true
            },
            new()
            {
                Text = "New deposit",
                Href = RouteHelpers.TransactionNew(TransactionConstants.Deposit),
                Icon = Icons.Material.Filled.ArrowRightAlt,
                FlipX = false
            },
            new()
            {
                Text = "New transfer",
                Href = RouteHelpers.TransactionNew(TransactionConstants.Transfer),
                Icon = Icons.Material.Filled.SyncAlt,
                FlipX = false
            },
            new()
            {
                Text = "New asset account",
                Href = RouteHelpers.AccountNew(AccountTypeConstants.Asset),
                Icon = Icons.Material.Filled.QuestionMark,
                FlipX = false
            },
            new()
            {
                Text = "New expense account",
                Href = RouteHelpers.AccountNew(AccountTypeConstants.Expense),
                Icon = Icons.Material.Filled.QuestionMark,
                FlipX = false
            },
            new()
            {
                Text = "New revenue account",
                Href = RouteHelpers.AccountNew(AccountTypeConstants.Revenue),
                Icon = Icons.Material.Filled.QuestionMark,
                FlipX = false
            }
        };
    }
}
