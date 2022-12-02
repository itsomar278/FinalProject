using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.FavoriteRepository
{
    public class FavouriteRepository : Repository<Favorite>, IFavouriteRepository
    {
        public FavouriteRepository(DbContext dbContext) : base(dbContext)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
    }
}
