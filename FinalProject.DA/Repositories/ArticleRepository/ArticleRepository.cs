using DataAcess;
using DataAcess.Entites;
using DataAcess.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DataAcess.Repositories.ArticleRepository
{
    public class ArticleRepository : Repository<Articles>, IArticleRepository
    {
        public ArticleRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Articles>> GetArticlesAsync(string? title, string? searchQuery, int pageNumber, int pageSize)
        {
            var articles = _DbContext.Set<Articles>().AsQueryable();

            if (!string.IsNullOrEmpty(title))
                articles = articles.Where(a => a.Title.Contains(title));

            if (!string.IsNullOrEmpty(searchQuery))
                articles = articles.Where(a => a.Content.Contains(searchQuery));

            articles = articles.OrderBy(a => a.Title)
                              .Skip(pageSize * (pageNumber - 1))
                              .Take(pageSize);

            return await articles.ToListAsync();
        }
    }
}
