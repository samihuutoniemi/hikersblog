using HikersBlog.Components;
using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Misc;
using HikersBlog.Misc.ExternalLoginHandlers;
using HikersBlog.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace HikersBlog.Pages;

public class ArticleBase : ComponentBase
{
    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    public string Username { get; set; }

    [Parameter]
    public string Blogname { get; set; }

    [Parameter]
    public string UrlName { get; set; }

    [Inject]
    public IProfileRepository ProfileRepository { get; set; }

    [Inject]
    public IBlogRepository BlogRepository { get; set; }

    [Inject]
    public IArticleRepository ArticleRepository { get; set; }

    [Inject]
    public IExternalUserRepository ExternalUserRepository { get; set; }

    [Inject]
    public FacebookHandler FacebookHandler { get; set; }

    [Inject]
    public GoogleHandler GoogleHandler { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ProtectedLocalStorage ProtectedLocalStore { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    public Domain.Models.Article Article { get; private set; }

    public CommentContext CommentContext { get; set; } = new();

    public InputTextAreaResponsive CommentTextArea { get; set; }

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
        Article = ArticleRepository.GetArticle(blog.Id, UrlName, "Hike");

        ApplicationState.IsOwnBlog = ApplicationState.UserId == blog.UserId;

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var storedUserId = await ProtectedLocalStore.GetAsync<string>("userId");
            if (storedUserId.Success)
            {
                ApplicationState.ExternalUser = ExternalUserRepository.GetByUserId(storedUserId.Value);
                StateHasChanged();
            }

        }
        await base.OnAfterRenderAsync(firstRender);
    }


    protected void GoToDay(string article)
    {
        NavigationManager.NavigateTo($"{Blogname}/{article}", true);
    }

    protected void NewArticle()
    {
        NavigationManager.NavigateTo($"{Blogname}/edit/{UrlName}");
    }

    protected void EditArticle()
    {
        NavigationManager.NavigateTo($"{Blogname}/{UrlName}/edit");
    }

    protected void LoginFacebook()
    {
        FacebookHandler.Login();
    }

    protected void LoginGoogle()
    {
        GoogleHandler.Login();
    }

    protected void LikeArticle()
    {
        if (ApplicationState.ExternalUser != null)
        {
            var like = !Article.Likes.Any(l => l.ExternalUserId == ApplicationState.ExternalUser.Id);

            ArticleRepository.LikeArticle(Article.Id, ApplicationState.ExternalUser.Id, like);
        }
    }

    protected void LikeComment(int commentId)
    {
        if (ApplicationState.ExternalUser != null)
        {
            var comment = Article.Comments.FirstOrDefault(c => c.Id == commentId);
            var like = !comment.Likes.Any(l => l.ExternalUserId == ApplicationState.ExternalUser.Id);

            ArticleRepository.LikeComment(commentId, ApplicationState.ExternalUser.Id, like);
        }
    }

    protected async void SetCommentContext(int? articleId, int? commentId)
    {
        if (ApplicationState.ExternalUser != null)
        {
            CommentContext.ArticleId = articleId;
            CommentContext.CommentId = commentId;


            await JS.InvokeVoidAsync("jsfunction.focusElement", "comment-textarea");
            // Does not seem to work!
            //if (CommentTextArea.Element != null)
            //{
            //    CommentTextArea.Element.Value.FocusAsync();
            //}
        }
    }

    protected void Comment()
    {
        if (ApplicationState.ExternalUser != null)
        {
            if (CommentContext.CommentId == null && CommentContext.ArticleId == null)
            {
                CommentContext.ArticleId = Article.Id;
            }

            if (string.IsNullOrEmpty(CommentContext.Content))
            {
                return;
            }

            var comment = new Domain.Models.Comment
            {
                ArticleId = Article.Id,
                ExternalUserId = ApplicationState.ExternalUser.Id,
                Content = CommentContext.Content,
                ParentId = CommentContext.CommentId,
                Timestamp = DateTime.Now
            };

            ArticleRepository.SaveComment(comment);

            CommentContext.ArticleId = null;
            CommentContext.CommentId = null;
            CommentContext.Content = null;
        }
    }

    protected async Task DisconnectExternalLogin()
    {
        var storedUserId = await ProtectedLocalStore.GetAsync<string>("userId");
        if (storedUserId.Success)
        {
            await ProtectedLocalStore.DeleteAsync("userId");

            ApplicationState.ExternalUser = null;

            StateHasChanged();
        }
    }

    protected async Task ViewPrivacyPolicy()
    {
        var source = ApplicationState.ExternalUser?.Source;

        if (source == "Facebook")
        {
            NavigationManager.NavigateTo("/facebook/privacypolicy");
        }
    }

    protected async Task ForgetMe()
    {
        var source = ApplicationState.ExternalUser?.Source;

        if (source == "Facebook")
        {
            ExternalUserRepository.Forget(ApplicationState.ExternalUser);
            await DisconnectExternalLogin();
            //NavigationManager.NavigateTo("/facebook/privacypolicy");
        }
    }
}
