using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Misc;
using HikersBlog.Storage;
using Microsoft.AspNetCore.Components;

namespace HikersBlog.Pages;

public class BlogIndexBase : ComponentBase
{
    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    public string Username { get; set; }

    [Parameter]
    public string Blogname { get; set; }

    [Inject]
    public IProfileRepository ProfileRepository { get; set; }

    [Inject]
    public IBlogRepository BlogRepository { get; set; }

    [Inject]
    public IArticleRepository ArticleRepository { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public Domain.Models.Article Article { get; private set; }

    public IEnumerable<Domain.Models.Article> BlogArticles { get; set; }

    protected override async Task OnInitializedAsync()
    {

        Username = "fjallaventyret";

        if (ApplicationState.Profile == null)
        {
            ApplicationState.Profile = ProfileRepository.GetProfile(Username);
        }

        if (ApplicationState.Blogs == null)
        {
            ApplicationState.Blogs = BlogRepository.GetBlogs(ApplicationState.Profile.UserId);
        }   

        var blog = ApplicationState.Blogs.FirstOrDefault(b => b.UrlName == Blogname);
        Article = ArticleRepository.GetArticle(blog.Id, null, "Index");

        BlogArticles = ArticleRepository.GetArticles(blog.Id);

        ApplicationState.IsOwnBlog = ApplicationState.UserId == blog.UserId;

        await base.OnInitializedAsync();
    }

    protected override Task OnParametersSetAsync()
    {
        if (NavigationManager.Uri.Replace(NavigationManager.BaseUri, string.Empty).StartsWith("blog/"))
        {
            NavigationManager.NavigateTo($"{Blogname}", true);
        }

        return base.OnParametersSetAsync();
    }
}
