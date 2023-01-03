using DataAcess.Entites;
using DataAcess.Repositories;

namespace DataAcess.Repositories.ArticleRepository
{
    public interface IArticleRepository : IRepository<Articles>
    {
        Task<IEnumerable<Articles>> GetArticlesAsync(string? title, string? searchQuery, int pageNumber, int pageSize);
    }
}
