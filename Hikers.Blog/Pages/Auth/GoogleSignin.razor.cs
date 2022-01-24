using HikersBlog.Domain.Misc;
using HikersBlog.Misc.ExternalLoginHandlers;
using Microsoft.AspNetCore.Components;

namespace HikersBlog.Pages;

public class GoogleSigninBase : ComponentBase
{
    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    [Inject]
    public GoogleHandler GoogleHandler { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GoogleHandler.HandleRedirect(ApplicationState);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
