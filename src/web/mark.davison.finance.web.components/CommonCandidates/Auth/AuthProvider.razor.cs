﻿namespace mark.davison.finance.web.components.CommonCandidates.Auth;

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
        var bffRoot = await _jsRuntime.InvokeAsync<string>("GetBffUri");

        if (string.IsNullOrEmpty(bffRoot))
        {
            Console.WriteLine("GetBffUri returned an empty/null value.");
        }
        else
        {
            AuthenticationConfig.SetBffBase(bffRoot);
        }

        await AuthenticationContext.ValidateAuthState();
    }
}
