namespace HikersBlog.Domain.Models;

public class Article
{
    public int Id { get; set; }
    public int BlogId { get; set; }
    public string UrlName { get; set; }
    public bool IsFrontPage { get; set; }
    public string Type { get; set; }
    public string PreviousArticle { get; set; }
    public string NextArticle { get; set; }
    public DateTime Timestamp { get; set; }

    public IEnumerable<ArticleEntry> Entries { get; set; }
    public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
    public IEnumerable<ArticleLike> Likes { get; set; } = new List<ArticleLike>();

    public bool Equals(Article other)
    {
        if (other is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (Object.ReferenceEquals(this, other))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (this.GetType() != other.GetType())
        {
            return false;
        }

        // Return true if the fields match.
        return UrlName == other.UrlName
            && IsFrontPage == other.IsFrontPage
            && Type == other.Type
            && PreviousArticle == other.PreviousArticle
            && NextArticle == other.NextArticle;
    }

    public override int GetHashCode() => $"{UrlName}-{IsFrontPage}-{Type}-{PreviousArticle}-{NextArticle}".GetHashCode();
}
