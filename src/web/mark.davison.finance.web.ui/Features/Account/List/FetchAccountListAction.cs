namespace mark.davison.finance.web.ui.Features.Account.List;

public class FetchAccountListAction : IAction<FetchAccountListAction>
{
    public FetchAccountListAction(bool showActive)
    {
        ShowActive = showActive;
    }

    public bool ShowActive { get; }
}
