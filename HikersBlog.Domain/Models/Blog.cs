namespace HikersBlog.Domain.Models;

public class Blog
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string UrlName { get; set; }
    public bool IsDefault { get; set; }
}
