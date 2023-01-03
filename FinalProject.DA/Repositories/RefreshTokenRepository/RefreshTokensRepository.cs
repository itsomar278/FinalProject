using DataAcess.Entites;
using Microsoft.EntityFrameworkCore;

namespace DataAcess.Repositories.RefreshTokenRepository
{
    public class RefreshTokensRepository : Repository<RefreshTokens>, IRefreshTokenRepository
    {
        public RefreshTokensRepository(ProjectDbContext dbContext) : base(dbContext)
        {
        }
    }
}
