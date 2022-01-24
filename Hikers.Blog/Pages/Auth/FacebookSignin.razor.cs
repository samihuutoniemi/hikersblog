using HikersBlog.Domain.Misc;
using HikersBlog.Misc.ExternalLoginHandlers;
using Microsoft.AspNetCore.Components;

namespace HikersBlog.Pages;

public class FacebookSigninBase : ComponentBase
{
    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    [Inject]
    public FacebookHandler FacebookHandler { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FacebookHandler.HandleRedirect(ApplicationState);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
