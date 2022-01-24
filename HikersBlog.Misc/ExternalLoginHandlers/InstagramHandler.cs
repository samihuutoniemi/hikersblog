using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Misc;
using HikersBlog.Domain.Misc.Authentication.Facebook;
using HikersBlog.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace HikersBlog.Misc.ExternalLoginHandlers;

public class InstagramHandler
{
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly IExternalUserRepository _externalUserRepository;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public InstagramHandler(IConfiguration configuration
        , NavigationManager navigationManager
        , ProtectedLocalStorage protectedLocalStorage
        , IExternalUserRepository externalUserRepository)
    {
        _navigationManager = navigationManager;
        _protectedLocalStorage = protectedLocalStorage;
        _externalUserRepository = externalUserRepository;

        _clientId = configuration["Instagram:ClientId"];
        _clientSecret = configuration["Instagram:ClientSecret"];

        // NOT YET IMPLEMENTED!
    }

    public void Login()
    {
        //using var client = new HttpClient();

        //var url = "https://www.facebook.com/v11.0/dialog/oauth";
        //var redirectUri = $"{_navigationManager.BaseUri}signin-facebook";
        //var state = _navigationManager.Uri;

        //var uri = $"{url}?client_id={_clientId}&scope=email&response_type=code&redirect_uri={redirectUri}&state={state}";

        //_navigationManager.NavigateTo(uri);
    }

    public async Task HandleRedirect(ApplicationState applicationState)
    {
        //var uri = _navigationManager.Uri;

        //var code = uri.Replace(_navigationManager.BaseUri, string.Empty)
        //    .Split("code=").Last()
        //    .Split("state=").First()
        //    .TrimEnd('&');

        //var state = uri.Replace(_navigationManager.BaseUri, string.Empty)
        //    .Split("state=").Last()
        //    .Split("#").First();

        //var url = "https://graph.facebook.com/v13.0/oauth/access_token";
        //var redirectUri = $"{_navigationManager.BaseUri}signin-facebook";

        //var tokenUri = $"{url}?client_id={_clientId}&client_secret={_clientSecret}&redirect_uri={redirectUri}&code={code}";

        //var client = new HttpClient();
        //var accessTokenResponse = await client.GetStringAsync(tokenUri);

        //var accessToken = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);

        //var userUri = $"https://graph.facebook.com/v13.0/me?fields=id,name,email,picture&access_token={accessToken.access_token}&debug=all&format=json&method=get&pretty=0&suppress_http_code=1&transport=cors";
        //var userResponse = await client.GetStringAsync(userUri);
        //var user = JsonSerializer.Deserialize<FacebookUser>(userResponse);

        //var externalUser = new ExternalUser
        //{
        //    Source = "Facebook",
        //    ExternalId = user.id,
        //    Email = user.email,
        //    Name = user.name,
        //    ImageUrl = user.picture.data.url,
        //    AccessToken = accessToken.access_token,
        //    AccessTokenExpiresAt = DateTime.Now.AddSeconds(accessToken.expires_in)
        //};

        //applicationState.ExternalUser = externalUser;
        //externalUser = _externalUserRepository.Save(externalUser);

        //await _protectedLocalStorage.SetAsync("userId", $"{externalUser.Id}|{externalUser.ExternalId}");

        //var returnUri = WebUtility.UrlDecode(state);
        //_navigationManager.NavigateTo(returnUri);
    }
}
