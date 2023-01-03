using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.FavoriteRepository
{
    public class FavouriteRepository : Repository<Favorite>, IFavouriteRepository
    {
        public FavouriteRepository(ProjectDbContext dbContext) : base(dbContext)
        {
        }

        override
           public Task<Favorite> GetAsync(int id)
        {
            throw new ArgumentException("unsupported opearation");
        }
    }
}
