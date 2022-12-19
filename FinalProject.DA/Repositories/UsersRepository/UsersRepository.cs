using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.UsersRepository
{
    public class UsersRepository : Repository<Users>, IUsersRepository
    {
        public UsersRepository(ProjectDbContext projectDbContext) : base(projectDbContext)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
        public async Task<Users> FindByEmailAsync(string email)
        {
            var user = await _DbContext.Set<Users>().SingleOrDefaultAsync(user => user.UserEmail == email);
            return user;
        }
        public async void UpdateUserRefreshToken(int userId ,int refreshTokenId)
        {
            var user = await GetAsync(userId);
            if (refreshTokenId == 0)
            {
                user.RefreshTokenId = null;
            }
            else
            {
                user.RefreshTokenId = refreshTokenId;
            }
        }
        public async Task<IEnumerable<Users>> GetUsersAsync(string? searchQuery, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                var users = (await GetAllAsync()).OrderBy(u => u.UserName).Skip(pageSize * (pageNumber - 1)).Take(pageSize); 
                return users;
            } 
            else
            {
                var users = (await FindAsync(u=>u.UserName.Contains(searchQuery))).OrderBy(u => u.UserName).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                return users;
            }
        }
    }
}
