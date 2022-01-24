using System.ComponentModel.DataAnnotations.Schema;

namespace HikersBlog.Domain.Models;

public class CommentLike
{
    public int Id { get; set; }
    public int CommentId { get; set; }
    public int ExternalUserId { get; set; }
    [ForeignKey(nameof(ExternalUserId))]
    public ExternalUser ExternalUser { get; set; }
}
