namespace HikersBlog.Domain.Misc;

public class CommentContext
{
    public int? ArticleId { get; set; }
    public int? CommentId { get; set; }
    public string Content { get; set; }
}
