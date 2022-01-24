using HikersBlog.DAL.Context;
using HikersBlog.DAL.Interface;
using HikersBlog.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HikersBlog.DAL;

public class ArticleRepository : IArticleRepository
{
    private readonly ApplicationDbContext _context;

    public ArticleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Article GetArticle(int articleId)
    {
        var result = (from article in _context.Articles.Include(a => a.Entries).Include(a => a.Likes).Include(a => a.Comments).ThenInclude(c => c.Likes)
                      where article.Id == articleId
                      //select new Article
                      //{
                      //    Id = article.Id,
                      //    BlogId = article.BlogId,
                      //    IsFrontPage = article.IsFrontPage,
                      //    Type = article.Type,
                      //    UrlName = article.UrlName,
                      //    PreviousArticle = article.PreviousArticle,
                      //    NextArticle = article.NextArticle,
                      //    Entries = article.Entries.Select(e => new ArticleEntry
                      //    {
                      //        Id = e.Id,
                      //        ArticleId = e.Id,
                      //        Type = e.Type,
                      //        Priority = e.Priority,
                      //        Data = e.Data,
                      //        SubData = e.SubData
                      //    }),
                      //}
                      select article
                      ).FirstOrDefault();

        return result;
    }

    public IEnumerable<Article> GetArticles(int blogId)
    {
        var result = (from article in _context.Articles.Include(a => a.Entries)
                      where article.BlogId == blogId
                      //select new Article
                      //{
                      //    Id = article.Id,
                      //    BlogId = article.BlogId,
                      //    IsFrontPage = article.IsFrontPage,
                      //    Type = article.Type,
                      //    UrlName = article.UrlName,
                      //    PreviousArticle = article.PreviousArticle,
                      //    NextArticle = article.NextArticle,
                      //    Entries = article.Entries.Select(e => new ArticleEntry
                      //    {
                      //        Id = e.Id,
                      //        ArticleId = e.Id,
                      //        Type = e.Type,
                      //        Priority = e.Priority,
                      //        Data = e.Data,
                      //        SubData = e.SubData
                      //    }),
                      //}
                      select article
                      ).ToList();

        return result;
    }

    public Article GetArticle(int blogId, string articleUrlName, string type)
    {
        //var result = (from article in _context.Articles.Include(a => a.Entries).Include(a => a.Likes)
        //              where article.BlogId == blogId
        //              && (articleUrlName == null && article.UrlName == null || (article.UrlName.ToLower() == (articleUrlName ?? "").ToLower() || articleUrlName == null && article.IsFrontPage == true))
        //              select new Article
        //              {
        //                  Id = article.Id,
        //                  BlogId = article.BlogId,
        //                  IsFrontPage = article.IsFrontPage,
        //                  Type = article.Type,
        //                  UrlName = article.UrlName,
        //                  PreviousArticle = article.PreviousArticle,
        //                  NextArticle = article.NextArticle,
        //                  Entries = article.Entries.Select(e => new ArticleEntry
        //                  {
        //                      Id = e.Id,
        //                      ArticleId = e.Id,
        //                      Type = e.Type,
        //                      Priority = e.Priority,
        //                      Data = e.Data,
        //                      SubData = e.SubData
        //                  }),
        //                  Comments = (from comment in _context.Comments
        //                              join eu1 in _context.ExternalUsers on comment.ExternalUserId equals eu1.Id
        //                              join __parentComment in _context.Comments on comment.ParentId equals __parentComment.Id into _parentComment
        //                              from parentComment in _parentComment.DefaultIfEmpty()
        //                              join __eu2 in _context.ExternalUsers on parentComment.ExternalUserId equals __eu2.Id into _eu2
        //                              from eu2 in _eu2.DefaultIfEmpty()
        //                              where comment.ArticleId == article.Id
        //                              select new Comment
        //                              {
        //                                  Id = comment.Id,
        //                                  ArticleId = comment.ArticleId,
        //                                  ParentId = comment.ParentId,
        //                                  Content = comment.Content,
        //                                  Timestamp = comment.Timestamp,
        //                                  ExternalUser = new ExternalUser
        //                                  {
        //                                      Id = eu1.Id,
        //                                      Name = eu1.Name,
        //                                  }
        //                              })
        //              }).FirstOrDefault();


        var result = (from article in _context.Articles.Include(a => a.Entries).Include(a => a.Likes).ThenInclude(l => l.ExternalUser)
                      where article.BlogId == blogId
                      && (articleUrlName == null && article.UrlName == null || (article.UrlName.ToLower() == (articleUrlName ?? "").ToLower() || articleUrlName == null && article.IsFrontPage == true))
                      && (type == null || article.Type == type)
                      select article).FirstOrDefault();

        if (result != null)
        {
            var comments = _context.Comments.Include(c => c.ExternalUser).Include(c => c.Likes).ThenInclude(l => l.ExternalUser).Where(c => c.ArticleId == result.Id).ToList();

            foreach (var comment in comments.Where(c => c.ParentId != null))
            {
                comment.ParentCommentExternalUser = comments.FirstOrDefault(c => c.Id == comment.ParentId).ExternalUser;
            }

            result.Comments ??= comments;
        }

        return result;
    }

    public Article SaveArticle(Article article)
    {
        // Ny artikel
        if (article.Id == 0)
        {
            article.Timestamp = DateTime.Now;
            _context.Articles.Add(article);
            _context.AddRange(article.Entries);
        }
        else
        {
            var currentArticle = GetArticle(article.Id);

            if (!currentArticle.Equals(article))
            {
                currentArticle.UrlName = article.UrlName;
                currentArticle.IsFrontPage = article.IsFrontPage;
                currentArticle.Type = article.Type;
                currentArticle.PreviousArticle = article.PreviousArticle;
                currentArticle.NextArticle = article.NextArticle;
            }

            currentArticle.Timestamp = DateTime.Now;

            _context.SaveChanges();

            foreach (var entry in article.Entries)
            {
                var currentEntry = currentArticle.Entries.FirstOrDefault(ae => ae.Id == entry.Id);

                if (currentEntry == null)
                {
                    currentEntry = new ArticleEntry();
                }

                if (!currentEntry.Equals(entry))
                {
                    currentEntry.ArticleId = entry.ArticleId;
                    currentEntry.Type = entry.Type;
                    currentEntry.Priority = entry.Priority;
                    currentEntry.Data = entry.Data;
                    currentEntry.SubData = entry.SubData;
                }
            }

        }

        _context.SaveChanges();

        return article;
    }

    public void SetArticleNext(int articleId, string next)
    {
        var article = _context.Articles.FirstOrDefault(a => a.Id == articleId);

        if (article != null)
        {
            article.NextArticle = next;

            _context.SaveChanges();
        }
    }

    public void LikeArticle(int articleId, int externalUserId, bool like)
    {
        var currentLike = _context.ArticleLikes.FirstOrDefault(al => al.ArticleId == articleId && al.ExternalUserId == externalUserId);

        if (currentLike != null && !like)
        {
            _context.ArticleLikes.Remove(currentLike);
        }
        else
        {
            _context.ArticleLikes.Add(new ArticleLike { ArticleId = articleId, ExternalUserId = externalUserId });
        }

        _context.SaveChanges();
    }

    public void LikeComment(int commentId, int externalUserId, bool like)
    {
        var currentLike = _context.CommentLikes.FirstOrDefault(cl => cl.CommentId == commentId && cl.ExternalUserId == externalUserId);

        if (currentLike != null && !like)
        {
            _context.CommentLikes.Remove(currentLike);
        }
        else
        {
            _context.CommentLikes.Add(new CommentLike { CommentId = commentId, ExternalUserId = externalUserId });
        }

        _context.SaveChanges();
    }

    public void SaveComment(Comment comment)
    {
        _context.Comments.Add(comment);
        _context.SaveChanges();
    }

    public IEnumerable<string> GetAllImageNames()
    {
        var result = (from ae in _context.ArticleEntries
                      where new[] { "TitleImage", "Image" }.Contains(ae.Type)
                      select ae.Data).ToList();

        return result;
    }
}
