﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Security.Cryptography;
using static mark.davison.finance.common.server.Authentication.ZenoAuthenticationConstants;

namespace mark.davison.finance.common.server.Authentication;

[AllowAnonymous]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ZenoAuthOptions _zenoAuthOptions;

    public AuthController(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IServiceProvider serviceProvider,
        IOptions<ZenoAuthOptions> options
    )
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _serviceProvider = serviceProvider;
        _zenoAuthOptions = options.Value;
    }

    [HttpGet(ZenoRouteNames.LoginRoute)]
    public async Task<IActionResult> Login(CancellationToken cancellationToken)
    {
        using SHA256 mySHA256 = SHA256.Create();
        var verifier = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        var challenge = WebEncoders.Base64UrlEncode(mySHA256.ComputeHash(Encoding.ASCII.GetBytes(verifier)));
        var state = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

        var zenoAuthenticationSession = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IZenoAuthenticationSession>();
        await zenoAuthenticationSession.LoadSessionAsync(cancellationToken);
        zenoAuthenticationSession.SetString(SessionNames.Verifier, verifier);
        zenoAuthenticationSession.SetString(SessionNames.Challenge, challenge);
        zenoAuthenticationSession.SetString(SessionNames.State, state);
        zenoAuthenticationSession.SetString(SessionNames.RedirectUri, _httpContextAccessor.HttpContext.Request.Query[ZenoQueryNames.RedirectUri]);
        zenoAuthenticationSession.Remove(SessionNames.AccessToken);
        zenoAuthenticationSession.Remove(SessionNames.RefreshToken);
        zenoAuthenticationSession.Remove(SessionNames.UserProfile);
        await zenoAuthenticationSession.CommitSessionAsync(cancellationToken);

        var queryParams = new Dictionary<string, string> {
                { OauthParamNames.ClientId,             _zenoAuthOptions.ClientId },
                { OauthParamNames.RedirectUri,          _zenoAuthOptions.BffOrigin + ZenoRouteNames.LoginCallbackRoute },
                { OauthParamNames.ResponseType,         OauthParams.Code },
                { OauthParamNames.Scope,                _zenoAuthOptions.Scope },
                { OauthParamNames.Audience,             _zenoAuthOptions.OpenIdConnectConfiguration.Issuer },
                { OauthParamNames.CodeChallengeMethod,  OauthParams.S256ChallengeMethod },
                { OauthParamNames.CodeChallenge,        challenge },
                { OauthParamNames.State,                state },
            };

        return WebUtilities.RedirectPreserveMethod(WebUtilities.CreateQueryUri(_zenoAuthOptions.OpenIdConnectConfiguration.AuthorizationEndpoint, queryParams).ToString());
    }

    [HttpGet(ZenoRouteNames.LoginCallbackRoute)]
    public async Task<IActionResult> LoginCallback(CancellationToken cancellationToken)
    {
        var zenoAuthenticationSession = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IZenoAuthenticationSession>();
        var error = _httpContextAccessor.HttpContext!.Request.Query[OauthQueryNames.Error];
        var error_description = _httpContextAccessor.HttpContext.Request.Query[OauthQueryNames.ErrorDescription];
        var state = _httpContextAccessor.HttpContext.Request.Query[OauthQueryNames.State];
        var code = _httpContextAccessor.HttpContext.Request.Query[OauthQueryNames.Code];

        if (!string.IsNullOrEmpty(error) || !string.IsNullOrEmpty(error_description))
        {
            var webErrorQueryParams = new Dictionary<string, string>
                {
                    { ZenoQueryNames.Error, error },
                    { ZenoQueryNames.ErrorDescription, error_description }
                };
            return WebUtilities.RedirectPreserveMethod(WebUtilities.CreateQueryUri(_zenoAuthOptions.WebOrigin + ZenoRouteNames.WebErrorRoute, webErrorQueryParams).ToString());
        }

        await zenoAuthenticationSession.LoadSessionAsync(cancellationToken);
        if (zenoAuthenticationSession.GetString(SessionNames.State) != state)
        {
            var webErrorQueryParams = new Dictionary<string, string>
                {
                    { ZenoQueryNames.Error, ZenoAuthErrors.AuthError },
                    { ZenoQueryNames.ErrorDescription, ZenoAuthErrors.StateMismatch }
                };
            return WebUtilities.RedirectPreserveMethod(WebUtilities.CreateQueryUri(_zenoAuthOptions.WebOrigin + ZenoRouteNames.WebErrorRoute, webErrorQueryParams).ToString());
        }

        var redirect = zenoAuthenticationSession.GetString(SessionNames.RedirectUri);

        var queryParams = new Dictionary<string, string> {
                { OauthParamNames.CodeVerifier,     zenoAuthenticationSession.GetString(SessionNames.Verifier)! },
                { OauthParamNames.Code,             code },
                { OauthParamNames.GrantType,        OauthParams.AuthorizationCodeGrantType },
                { OauthParamNames.ClientId,         _zenoAuthOptions.ClientId },
                { OauthParamNames.ClientSecret,     _zenoAuthOptions.ClientSecret },
                { OauthParamNames.RedirectUri,      _zenoAuthOptions.BffOrigin + ZenoRouteNames.LoginCallbackRoute }
            };
        AuthTokens? tokens;
        UserProfile? userProfile;
        var client = _httpClientFactory.CreateClient(AuthClientName);
        var message = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(_zenoAuthOptions.OpenIdConnectConfiguration.TokenEndpoint),
            Headers = { { HttpRequestHeader.ContentType.ToString(), WebUtilities.ContentType.FormUrlEncoded } },
            Content = new FormUrlEncodedContent(queryParams)
        };

        using (HttpResponseMessage responseMessage = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, _httpContextAccessor.HttpContext.RequestAborted))
        {
            tokens = await responseMessage.Content.ReadFromJsonAsync<AuthTokens>();
            if (tokens == null)
            {
                return new BadRequestObjectResult(await responseMessage.Content.ReadAsStringAsync());
            }
        }

        var userMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_zenoAuthOptions.OpenIdConnectConfiguration.UserInfoEndpoint),
            Headers = { { HttpRequestHeader.Authorization.ToString(), WebUtilities.CreateBearerHeaderValue(tokens.access_token!) } }
        };
        using (HttpResponseMessage responseMessage = await client.SendAsync(userMessage, HttpCompletionOption.ResponseHeadersRead, _httpContextAccessor.HttpContext.RequestAborted))
        {
            userProfile = await responseMessage.Content.ReadFromJsonAsync<UserProfile>();
            if (userProfile == null)
            {
                return new BadRequestObjectResult(await responseMessage.Content.ReadAsStringAsync());
            }
        }

        zenoAuthenticationSession.Remove(SessionNames.Verifier);
        zenoAuthenticationSession.Remove(SessionNames.Challenge);
        zenoAuthenticationSession.Remove(SessionNames.State);
        zenoAuthenticationSession.Remove(SessionNames.RedirectUri);
        zenoAuthenticationSession.SetString(SessionNames.AccessToken, tokens.access_token!);
        zenoAuthenticationSession.SetString(SessionNames.RefreshToken, tokens.refresh_token!);
        zenoAuthenticationSession.SetString(SessionNames.UserProfile, JsonSerializer.Serialize(userProfile));

        var customActions = _serviceProvider.GetService<ICustomZenoAuthenticationActions>();
        if (customActions != null)
        {
            await customActions.OnUserAuthenticated(userProfile, cancellationToken);
        }

        await zenoAuthenticationSession.CommitSessionAsync(cancellationToken);

        return WebUtilities.RedirectPreserveMethod(_zenoAuthOptions.WebOrigin + redirect);
    }

    [HttpGet(ZenoRouteNames.LogoutRoute)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var zenoAuthenticationSession = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IZenoAuthenticationSession>();
        var refreshToken = zenoAuthenticationSession.GetString(SessionNames.RefreshToken);
        var access_token = zenoAuthenticationSession.GetString(SessionNames.AccessToken);
        if (string.IsNullOrEmpty(refreshToken) ||
            string.IsNullOrEmpty(access_token))
        {
            return new RedirectResult(_zenoAuthOptions.WebOrigin);
        }

        zenoAuthenticationSession.Clear();
        await zenoAuthenticationSession.CommitSessionAsync(cancellationToken);

        var queryParams = new Dictionary<string, string> {
                { OauthParamNames.ClientId, _zenoAuthOptions.ClientId },
                { OauthParamNames.RefreshToken, refreshToken },
                { OauthParamNames.RedirectUri, _zenoAuthOptions.BffOrigin + ZenoRouteNames.LogoutCallbackRoute }
            };

        return WebUtilities.RedirectPreserveMethod(WebUtilities.CreateQueryUri(_zenoAuthOptions.OpenIdConnectConfiguration.EndSessionEndpoint, queryParams).ToString());
    }

    [HttpGet(ZenoRouteNames.LogoutCallbackRoute)]
    public IActionResult LogoutCallback(CancellationToken cancellationToken)
    {
        return WebUtilities.RedirectPreserveMethod(_zenoAuthOptions.WebOrigin);
    }

    [HttpGet(ZenoRouteNames.UserRoute)]
    public async Task<IActionResult> GetUser(CancellationToken cancellationToken)
    {
        var zenoAuthenticationSession = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IZenoAuthenticationSession>();
        var userString = zenoAuthenticationSession.GetString(SessionNames.UserProfile);
        var refreshToken = zenoAuthenticationSession.GetString(SessionNames.RefreshToken);
        if (string.IsNullOrEmpty(userString) ||
            string.IsNullOrEmpty(refreshToken))
        {
            zenoAuthenticationSession.Clear();
            await zenoAuthenticationSession.CommitSessionAsync(cancellationToken);
            return new StatusCodeResult(401);
        }

        var profile = JsonSerializer.Deserialize<UserProfile>(userString);
        if (profile == null)
        {
            return new StatusCodeResult(401);
        }

        return new OkObjectResult(profile);
    }
}