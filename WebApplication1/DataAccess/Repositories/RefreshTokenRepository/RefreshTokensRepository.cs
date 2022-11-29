using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.RefreshTokenRepository
{
    public class RefreshTokensRepository : Repository<RefreshTokens>, IRefreshTokenRepository
    {
        public RefreshTokensRepository(DbContext dbContext) : base(dbContext)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
    }
}
