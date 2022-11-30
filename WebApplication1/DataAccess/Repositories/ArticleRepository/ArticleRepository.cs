using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.ArticleRepository
{
    public class ArticleRepository : Repository<Articles>, IArticleRepository
    {
        public ArticleRepository(ProjectDbContext context) : base(context)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
        public IEnumerable<Articles> GetArticles(string? title, string? searchQuery)
        {
            if(string.IsNullOrEmpty(title) && string.IsNullOrEmpty(searchQuery))
            {
                return GetAll();
            }
           if (string.IsNullOrEmpty(searchQuery))
            {
                return Find(a => a.Title.Contains(title));
            }
           if(string.IsNullOrEmpty(title))
            {
                return Find(a => a.Content.Contains(searchQuery));
            }
            return Find(a => a.Content.Contains(searchQuery) && a.Title.Contains(title));
        }
    }
}
