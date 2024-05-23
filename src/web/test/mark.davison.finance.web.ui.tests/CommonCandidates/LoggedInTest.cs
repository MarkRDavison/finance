namespace mark.davison.finance.web.ui.tests.CommonCandidates;

public abstract class LoggedInTest : BaseTest
{
    protected override async Task OnTestInitialise()
    {
        await AuthenticationHelper.EnsureLoggedIn(CurrentPage);
    }
}
