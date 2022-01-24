using HikersBlog.Domain.Models;

namespace HikersBlog.DAL.Interface
{
    public interface IArticleRepository
    {
        IEnumerable<string> GetAllImageNames();
        Article GetArticle(int blogId, string articleUrlName, string type);
        Article GetArticle(int articleId);
        IEnumerable<Article> GetArticles(int blogId);
        void LikeArticle(int articleId, int externalUserId, bool like);
        void LikeComment(int commentId, int externalUserId, bool like);
        Article SaveArticle(Article article);
        void SaveComment(Comment comment);
        void SetArticleNext(int articleId, string next);
    }
}