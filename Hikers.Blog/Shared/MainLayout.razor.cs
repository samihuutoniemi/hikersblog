using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Misc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HikersBlog.Shared;

public class MainLayoutBase : LayoutComponentBase
{
    [Inject]
    AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public IProfileRepository ProfileRepository { get; set; }

    [Inject]
    public IBlogRepository BlogRepository { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public ApplicationState ApplicationState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ApplicationState = new ApplicationState();

        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userId = authenticationState.User.Claims.FirstOrDefault(c => c.Type.EndsWith("nameidentifier"))?.Value;

        if (userId != null)
        {
            var userIdGuid = Guid.Parse(userId);

            ApplicationState.Profile = ProfileRepository.GetProfile(userIdGuid);
            ApplicationState.Blogs = BlogRepository.GetBlogs(ApplicationState.Profile.UserId);
            ApplicationState.UserId = userIdGuid;
        }
        else
        {
            ApplicationState.Blogs = BlogRepository.GetBlogs(Guid.Parse("c9be0546-7ac2-4ef0-b3fb-9ccfc62dfd4d"));
        }

        await base.OnInitializedAsync();
    }
}


