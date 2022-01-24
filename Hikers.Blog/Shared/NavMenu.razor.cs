using HikersBlog.Domain.Misc;
using HikersBlog.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace HikersBlog.Shared;

public class NavMenuBase : ComponentBase
{
    protected bool collapseNavMenu = false;

    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    protected string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    public IEnumerable<Blog> Blogs { get; private set; }

    protected void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
