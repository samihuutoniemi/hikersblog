using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Misc;
using HikersBlog.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HikersBlog.Pages;

public class IndexBase : ComponentBase
{
    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (NavigationManager.Uri.Replace(NavigationManager.BaseUri, string.Empty) == ""
            && ApplicationState.Blogs != null)
        {
            var defaultBlog = ApplicationState.Blogs.FirstOrDefault(b => b.IsDefault);
            NavigationManager.NavigateTo($"/{defaultBlog.UrlName}");
        }

        await base.OnInitializedAsync();
    }
}
