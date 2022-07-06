﻿namespace mark.davison.finance.common.client.Authentication;

public partial class AuthenticationContext : ObservableObject, IAuthenticationContext
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthenticationConfig _authenticationConfig;
    private readonly NavigationManager _navigationManager;

    public AuthenticationContext(
        IHttpClientFactory httpClientFactory,
        IAuthenticationConfig authenticationConfig,
        NavigationManager navigationManager
    )
    {
        _httpClientFactory = httpClientFactory;
        _authenticationConfig = authenticationConfig;
        _navigationManager = navigationManager;
    }

    [ObservableProperty]
    private Guid _userId;
    [ObservableProperty]
    private bool _isAuthenticated;
    [ObservableProperty]
    private bool _isAuthenticating = true;
    [ObservableProperty]
    private UserProfile? _user;

    public async Task ValidateAuthState()
    {
        try
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_authenticationConfig.UserEndpoint)
            };

            var client = _httpClientFactory.CreateClient("API");
            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserProfile>(data);
                if (user != null)
                {
                    IsAuthenticated = true;
                    IsAuthenticating = false;
                    User = user;
                }
            }
            else
            {
                IsAuthenticated = false;
                IsAuthenticating = false;
                User = null;
            }
        }
        catch (Exception)
        {
            IsAuthenticated = false;
            IsAuthenticating = false;
            User = null;
        }

        if (!IsAuthenticated)
        {
            await Login();
        }
    }
    public async Task Login()
    {
        await Task.CompletedTask;
        var relative = _navigationManager.Uri.Replace(_navigationManager.BaseUri.Trim('/'), "");
        _navigationManager.NavigateTo(_authenticationConfig.LoginEndpoint + "?redirect_uri=" + relative);
    }
    public async Task Logout()
    {
        await Task.CompletedTask;
        _navigationManager.NavigateTo(_authenticationConfig.LogoutEndpoint);
    }
}