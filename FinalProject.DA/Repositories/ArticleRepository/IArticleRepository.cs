using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.ArticleRepository
{
    public interface IArticleRepository : IRepository<Articles>
    {
       IEnumerable<Articles> GetArticles(string? title, string? searchQuery , int pageNumber , int pageSize);
    } 
}
