using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Misc;
using HikersBlog.Domain.Misc.Authentication.Google;
using HikersBlog.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HikersBlog.Misc.ExternalLoginHandlers;

public class GoogleHandler
{
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly IExternalUserRepository _externalUserRepository;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public GoogleHandler(IConfiguration configuration
        , NavigationManager navigationManager
        , ProtectedLocalStorage protectedLocalStorage
        , IExternalUserRepository externalUserRepository)
    {
        _navigationManager = navigationManager;
        _protectedLocalStorage = protectedLocalStorage;
        _externalUserRepository = externalUserRepository;

        _clientId = configuration["Google:ClientId"];
        _clientSecret = configuration["Google:ClientSecret"];
    }

    public void Login()
    {
        using var client = new HttpClient();

        var url = "https://accounts.google.com/o/oauth2/v2/auth";
        var redirectUri = $"{_navigationManager.BaseUri}signin-google";
        var responseType = "code";
        var includeGrantedScopes = "true";
        var state = _navigationManager.Uri;
        var scope = "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";

        var uri = $"{url}?client_id={_clientId}&redirect_uri={redirectUri}&response_type={responseType}&include_granted_scopes={includeGrantedScopes}&state={state}&scope={scope}&access_type=offline";

        _navigationManager.NavigateTo(uri);
    }

    public async Task HandleRedirect(ApplicationState applicationState)
    {
        var uri = _navigationManager.Uri;

        var authResponseValues = _navigationManager.Uri
             .Replace(_navigationManager.BaseUri, string.Empty)
             .Replace("signin-google?", string.Empty)
             .Split("&").ToList()
             .ToDictionary(x => x.Split("=").First(), x => x.Split("=").Last());

        var redirectBackPage = authResponseValues["state"];

        if (authResponseValues.ContainsKey("code"))
        {
            using HttpClient client = new HttpClient();

            var url = "https://oauth2.googleapis.com/token";
            var code = authResponseValues["code"];
            var state = authResponseValues["state"];

            code = WebUtility.UrlDecode(code);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                            { "client_id", _clientId },
                            { "client_secret", _clientSecret },
                            { "grant_type", "authorization_code" },
                            { "code", code },
                            { "redirect_uri", $"{_navigationManager.BaseUri}signin-google" },
                            { "access_type", "offline" },
                            { "scope", "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email"}
                        });

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var lines = result[1..^1].Trim().Split("\n").Select(s => s.Trim().TrimEnd(',')).ToList();

            var accessToken = lines.FirstOrDefault(l => l.Contains("access_token")).Split(":").LastOrDefault().Trim().Replace("\"", string.Empty);
            var refreshToken = lines.FirstOrDefault(l => l.Contains("refresh_token"))?.Split(":")?.LastOrDefault()?.Trim()?.Replace("\"", string.Empty);
            var tokenExpiryTime = DateTime.Now.AddSeconds(int.Parse(lines.FirstOrDefault(l => l.Contains("expires_in")).Split(":").LastOrDefault().Trim().Replace("\"", string.Empty)));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var peopleApiUrl = $"https://content-people.googleapis.com/v1/people/me?personFields=names,coverPhotos,emailAddresses";
            var peopleApiResponse = await client.GetAsync(peopleApiUrl);
            var peopleApiResult = await peopleApiResponse.Content.ReadAsStringAsync();
            
            var user = JsonSerializer.Deserialize<GooglePeopleProfile>(peopleApiResult);

            var externalUser = new ExternalUser
            {
                Source = "Google",
                ExternalId = user.resourceName.Split('/').LastOrDefault(),
                Email = user.emailAddresses.FirstOrDefault()?.value,
                Name = user.names.FirstOrDefault()?.displayName,
                ImageUrl = user.coverPhotos.FirstOrDefault()?.url,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = tokenExpiryTime
            };

            applicationState.ExternalUser = externalUser;
            externalUser = _externalUserRepository.Save(externalUser);

            await _protectedLocalStorage.SetAsync("userId", $"{externalUser.Id}|{externalUser.ExternalId}");

            var returnUri = WebUtility.UrlDecode(state);
            _navigationManager.NavigateTo(returnUri);
        }
        else
        {
            var accessToken = authResponseValues["access_token"];
            var tokenExpiryTime = DateTime.Now.AddSeconds(int.Parse(authResponseValues["expires_in"]));

            _navigationManager.NavigateTo(redirectBackPage);
        }
    }
}
