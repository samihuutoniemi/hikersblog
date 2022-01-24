using System.ComponentModel.DataAnnotations.Schema;

namespace HikersBlog.Domain.Models;

public class Comment
{
    public int Id { get; set; }
    public int ExternalUserId { get; set; }
    public int ArticleId { get; set; }
    public int? ParentId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public IEnumerable<CommentLike> Likes { get; set; } = new List<CommentLike>();
    [ForeignKey(nameof(ExternalUserId))]
    public ExternalUser ExternalUser { get; set; }
    [NotMapped]
    public ExternalUser ParentCommentExternalUser { get; set; }
}
