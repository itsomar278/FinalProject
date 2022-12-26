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
        public async Task<IEnumerable<Articles>> GetArticlesAsync(string? title, string? searchQuery, int pageNumber, int pageSize)
        {
            IEnumerable<Articles> articles;

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(searchQuery))
            {
                articles = await _DbContext.Set<Articles>().OrderBy(a => a.Title).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                return articles;
            }

            if (!string.IsNullOrEmpty(title) && string.IsNullOrEmpty(searchQuery))
            {
                articles = await _DbContext.Set<Articles>().Where(a => a.Title.Contains(title)).OrderBy(a => a.Title).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                return articles;
            }

            if (string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(searchQuery))
            {
                articles = await _DbContext.Set<Articles>().Where(a => a.Content.Contains(searchQuery))
                    .OrderBy(a => a.Title).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
                return articles;
            }

            articles = await _DbContext.Set<Articles>().Where(a => a.Content.Contains(searchQuery) && a.Title.Contains(title))
                    .OrderBy(a => a.Title).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();

            return articles;
        }
    }
}
