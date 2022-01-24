using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Misc;
using HikersBlog.Domain.Models;
using HikersBlog.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace HikersBlog.Pages;

// TODO:

// More Login providers for comments
// Email on new article

public class EditArticleBase : ComponentBase
{
    [CascadingParameter]
    public ApplicationState ApplicationState { get; set; }

    public string Username { get; set; }

    [Parameter]
    public string Blogname { get; set; }

    [Parameter]
    public string UrlName { get; set; }

    [Parameter]
    public string FromArticle { get; set; }

    [Inject]
    public IProfileRepository ProfileRepository { get; set; }

    [Inject]
    public IBlogRepository BlogRepository { get; set; }

    [Inject]
    public IArticleRepository ArticleRepository { get; set; }

    [Inject]
    public AzureBlobStorageHandler AzureBlobStorageHandler { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public Domain.Models.Article Article { get; private set; }
    public Domain.Models.Article PreviousArticle { get; private set; }
    public Domain.Models.Article NextArticle { get; private set; }

    public ArticleEntry DateEntry { get; set; } = new ArticleEntry { Type = "Date", Data = DateTime.Today.ToString("d MMMM yyyy") };
    public ArticleEntry RouteEntry { get; set; } = new ArticleEntry() { Type = "Route" };
    public ArticleEntry DistanceEntry { get; set; } = new ArticleEntry() { Type = "Distance" };
    public ArticleEntry WeatherEntry { get; set; } = new ArticleEntry() { Type = "Weather" };
    public (ArticleEntry Entry, IBrowserFile File) TitleImageEntry { get; set; } = (new ArticleEntry() { Type = "TitleImage" }, null);

    public List<ArticleEntryUIObject> ArticleEntries { get; set; } = new List<ArticleEntryUIObject>();

    public bool IsSaving { get; set; }

    public void DoNothing()
    {
    }

    protected override async Task OnInitializedAsync()
    {
        if (!ApplicationState.IsLoggedIn)
        {
            return;
        }

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

        PreviousArticle = FromArticle != null
              ? ArticleRepository.GetArticle(blog.Id, FromArticle, "Hike")
              : new Domain.Models.Article { };

        if (Article == null)
        {
            Article = new Domain.Models.Article
            {
                BlogId = blog.Id,
                Type = "Hike",
                IsFrontPage = false,
                PreviousArticle = PreviousArticle.UrlName
                //Entries = new List<Domain.Models.ArticleEntry>()
            };

            if (PreviousArticle?.UrlName != null && Regex.IsMatch(PreviousArticle.UrlName, @"\d+$"))
            {
                var number = Regex.Match(PreviousArticle.UrlName, @"\d+$").Value;
                Article.UrlName = $"{PreviousArticle.UrlName.Replace(number, (int.Parse(number) + 1).ToString())}";
            }

            if (PreviousArticle != null)
            {
                try
                {
                    var previousDate = PreviousArticle.Entries.FirstOrDefault(e => e.Type == "Date");
                    var prevDateTime = DateTime.Parse(previousDate.Data);

                    DateEntry.Data = $"{prevDateTime.AddDays(1):dd MMMM yyyy}"; ;
                    DateEntry.SubData = $"{int.Parse(previousDate.SubData) + 1}";

                    var previousRoute = PreviousArticle.Entries.FirstOrDefault(e => e.Type == "Route");
                    RouteEntry.Data = previousRoute.SubData;
                }
                catch { }

            }
        }
        else
        {
            DateEntry = Article.Entries.FirstOrDefault(ae => ae.Type == "Date");
            RouteEntry = Article.Entries.FirstOrDefault(ae => ae.Type == "Route");
            DistanceEntry = Article.Entries.FirstOrDefault(ae => ae.Type == "Distance");
            WeatherEntry = Article.Entries.FirstOrDefault(ae => ae.Type == "Weather");
            TitleImageEntry = (Article.Entries.FirstOrDefault(ae => ae.Type == "TitleImage"), null);

            ArticleEntries = Article.Entries.Where(ae => ae.Priority > 0).OrderBy(ae => ae.Priority).Select((ae, i) => new ArticleEntryUIObject
            {
                Entry = ae,
                Index = i,
            }).ToList();
        }

        await base.OnInitializedAsync();
    }

    protected async Task LoadFile(InputFileChangeEventArgs e, int? index)
    {
        IsSaving = true;
        if (index == null)
        {
            TitleImageEntry = (TitleImageEntry.Entry, e.File);
            await ProcessAndUploadImage(e.File, e.File.Name);
            IsSaving = false;
        }
        else
        {
            var entry = ArticleEntries.FirstOrDefault(ae => ae.Index == index.Value);

            var entryIndex = ArticleEntries.IndexOf(entry);
            ArticleEntries.Remove(entry);

            entry.File = e.File;

            ArticleEntries.Insert(entryIndex, entry);

            await ProcessAndUploadImage(e.File, e.File.Name);
            IsSaving = false;

            StateHasChanged();
        }
    }

    protected void InsertEntry(string type)
    {
        var lastIndex = ArticleEntries.Any() ? ArticleEntries.LastOrDefault().Index : 0;
        var lastPriority = ArticleEntries.Any() ? ArticleEntries.Select(ae => ae.Entry.Priority).Max() : 0;

        ArticleEntries.Add(new ArticleEntryUIObject { Entry = new ArticleEntry { Type = type, Priority = lastPriority + 1 }, Index = lastIndex + 1 });
    }

    protected async Task Save()
    {
        IsSaving = true;

        var entries = new List<ArticleEntry>
        {
            DateEntry,
            RouteEntry,
            DistanceEntry,
            WeatherEntry
        };

        if ((TitleImageEntry.Entry.Data ?? TitleImageEntry.File?.Name) != null)
        {
            TitleImageEntry.Entry.Data = TitleImageEntry.Entry.Data ?? TitleImageEntry.File.Name;

            if (TitleImageEntry.File?.Name != null && TitleImageEntry.Entry?.Data != TitleImageEntry.File.Name)
            {
                TitleImageEntry.Entry.Data = TitleImageEntry.File.Name;
            }

            entries.Add(TitleImageEntry.Entry);
        }

        int index = 1;
        foreach (var entry in ArticleEntries)
        {
            entry.Entry.ArticleId = Article.Id;
            entry.Entry.Priority = index;

            if (entry.Entry.Type == "Header1")
            {
                entries.Add(entry.Entry);
            }
            else if (entry.Entry.Type == "Text")
            {
                //var editor = entry.Editor;
                //var html = await editor.Editor.GetHTML();

                //entry.Entry.Data = html;

                entries.Add(entry.Entry);
            }
            else if (entry.Entry.Type == "Image")
            {
                entry.Entry.Data = entry.File?.Name ?? entry.Entry.Data;

                entries.Add(entry.Entry);
            }
            else if (entry.Entry.Type == "Video")
            {
                var youtubeUrl = entry.Entry.Data;
                if (youtubeUrl.Contains("youtube.com"))
                {
                    var match = Regex.Match(youtubeUrl, @"(https?:\/\/)?(www.)?youtube.com\/watch\?v=([a-zA-Z0-9_-]+)");
                    entry.Entry.Data = match.Groups[^1].Value;
                }

                entries.Add(entry.Entry);
            }

            index++;
        }

        foreach (var entry in entries)
        {
            entry.Article = Article;
        }

        Article.Entries = entries;

        ArticleRepository.SaveArticle(Article);
        ArticleRepository.SetArticleNext(PreviousArticle.Id, Article.UrlName);

        //foreach (var entry in entries.Where(e => e.Type == "Image" || e.Type == "TitleImage"))
        //{
        //    var metaEntry = ArticleEntries.FirstOrDefault(ae => (ae.File?.Name ?? ae.Entry.Data) == entry.Data)
        //        ?? (entry.Type == "TitleImage" ? new ArticleEntryUIObject { File = TitleImageEntry.File } : null);

        //    if (metaEntry.File != null)
        //    {
        //        await ProcessAndUploadImage(metaEntry);
        //    }
        //}

        IsSaving = false;

        if (NavigationManager.Uri.EndsWith($"/edit/{FromArticle}"))
        {
            NavigationManager.NavigateTo($"{Blogname}/{Article.UrlName}/edit", true);
        }
    }

    private async Task ProcessAndUploadImage(IBrowserFile file, string filename)
    {
        Stream outStream;
        Image image = null;
        IImageFormat format;

        var name = $"{Username}/{Blogname}/{filename}";

        if (!filename.ToLower().EndsWith(".heic"))
        {
            (image, format) = await Image.LoadWithFormatAsync(file.OpenReadStream(maxAllowedSize: 50000000));

            var imageSize = image.Size();
            var isLandscape = imageSize.Width > image.Height;
            var size = isLandscape ? imageSize.Width : imageSize.Height;

            if (size > 5184)
            {
                var resizedImageStream = new MemoryStream();
                var sizeRatio = size / 3000m;

                image.Mutate(x => x.Resize((int)Math.Round(imageSize.Width / sizeRatio), (int)Math.Round(imageSize.Height / sizeRatio)));

                image.Save(resizedImageStream, format);
                resizedImageStream.Position = 0;

                outStream = resizedImageStream;
            }
            else
            {
                outStream = file?.OpenReadStream(maxAllowedSize: 50000000);
            }
        }
        else
        {
            outStream = file?.OpenReadStream(maxAllowedSize: 50000000);
        }

        await AzureBlobStorageHandler.UploadBlob(name, outStream);

        file = null;

        outStream.Dispose();
        image?.Dispose();
    }

    public void RemoveEntry(ArticleEntryUIObject entry)
    {
        ArticleEntries.Remove(entry);
    }

    public async Task RemoveUnusedImagesOnAzureBlobStorage()
    {
        var imageList = ArticleRepository.GetAllImageNames();
        AzureBlobStorageHandler.DeleteDeadBlobs(imageList);
    }

    //public void OnItemDrop(ArticleEntryUIObject entry)
    //{

    //}
}

public class ArticleEntryUIObject
{
    public ArticleEntry Entry { get; set; }
    public IBrowserFile File { get; set; }
    public int Index { get; set; }
}
