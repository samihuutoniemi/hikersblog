using System.ComponentModel.DataAnnotations.Schema;

namespace HikersBlog.Domain.Models;

public class ArticleEntry
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string Type { get; set; }
    public int Priority { get; set; }
    public string Data { get; set; }
    public string SubData { get; set; }

    [ForeignKey(nameof(ArticleId))]
    public Article Article { get; set; }

    public bool Equals(ArticleEntry other)
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
        return Type == other.Type
            && Priority == other.Priority
            && Data == other.Data
            && SubData == other.SubData;
    }

    public override int GetHashCode() => $"{Type}{Priority}-{Data}-{SubData}".GetHashCode();
}
