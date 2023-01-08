using DataAcess.Entites;
using Microsoft.EntityFrameworkCore;

namespace DataAcess.Repositories.FavoriteRepository
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
