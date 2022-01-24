using HikersBlog.DAL.Context;
using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Models;

namespace HikersBlog.DAL;

public class BlogRepository : IBlogRepository
{
    private readonly ApplicationDbContext _context;

    public BlogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Blog> GetBlogs(Guid userId)
    {
        var result = (from blog in _context.Blogs
                      where blog.UserId == userId
                      select blog).ToList();

        return result;
    }

    public IEnumerable<Blog> GetBlogs(string urlName)
    {
        var result = (from blog in _context.Blogs
                      where blog.UrlName == urlName
                      select blog).ToList();

        return result;
    }
}
