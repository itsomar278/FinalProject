using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.DataAccess;
using WebApplication1.Models.Entites;

namespace WebApplication1.Models.Repositories.ArticleRepository
{
    public class ArticleRepository : Repository<Articles> , IArticleRepository
    {
       public ArticleRepository(ProjectDbContext context) : base(context)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
    }
}
