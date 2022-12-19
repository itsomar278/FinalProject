using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.ArticleRepository
{
    public interface IArticleRepository : IRepository<Articles>
    {
       Task<IEnumerable<Articles>> GetArticlesAsync(string? title, string? searchQuery , int pageNumber , int pageSize);
    } 
}
