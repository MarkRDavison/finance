namespace mark.davison.finance.web.components.CommonCandidates.Auth;

public partial class AuthProvider
{
    [Parameter]
    public IAuthenticationConfig AuthenticationConfig { get; set; } = default!;

    [Inject]
    public IAuthenticationContext AuthenticationContext { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await AuthenticationContext.ValidateAuthState();
        await base.OnInitializedAsync();
    }
}
