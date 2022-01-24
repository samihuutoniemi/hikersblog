using System.ComponentModel.DataAnnotations.Schema;

namespace HikersBlog.Domain.Models;

public class ArticleLike
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int ExternalUserId { get; set; }
    [ForeignKey(nameof(ExternalUserId))]
    public ExternalUser ExternalUser { get; set; }
}
