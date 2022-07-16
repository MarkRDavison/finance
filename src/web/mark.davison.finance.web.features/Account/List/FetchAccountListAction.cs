namespace mark.davison.finance.web.features.Account.List;

public class FetchAccountListAction : IAction<FetchAccountListAction>
{
    public FetchAccountListAction(bool showActive)
    {
        ShowActive = showActive;
    }

    public bool ShowActive { get; }
}
