using HikersBlog.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HikersBlog.DAL.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleEntry> ArticleEntries { get; set; }
        public DbSet<ExternalUser> ExternalUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ArticleLike> ArticleLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
    }
}