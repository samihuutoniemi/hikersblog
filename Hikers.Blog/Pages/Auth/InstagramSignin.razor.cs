using HikersBlog.Domain.Misc;
using HikersBlog.Misc.ExternalLoginHandlers;
using Microsoft.AspNetCore.Components;

namespace HikersBlog.Pages;

public class InstagramSigninBase : ComponentBase
{
    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    [Inject]
    public InstagramHandler InstagramHandler { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InstagramHandler.HandleRedirect(ApplicationState);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
