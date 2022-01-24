using HikersBlog.Domain.Models;

namespace HikersBlog.DAL.Interface;

public interface IBlogRepository
{
    IEnumerable<Blog> GetBlogs(Guid userId);
    IEnumerable<Blog> GetBlogs(string urlName);
}
