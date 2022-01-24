using HikersBlog.Domain.Models;

namespace HikersBlog.Domain.Misc;

public class ApplicationState
{
    public Profile Profile { get; set; }
    public IEnumerable<Blog> Blogs { get; set; }
    public Guid? UserId { get; set; }
    public bool IsLoggedIn => UserId != null;
    public bool IsOwnBlog { get; set; }
    public ExternalUser ExternalUser { get; set; }
}
