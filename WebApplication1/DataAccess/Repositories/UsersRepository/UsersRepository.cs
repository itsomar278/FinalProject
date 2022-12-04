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
        public Users FindByEmail(string email)
        {
            var user = _DbContext.Set<Users>().SingleOrDefault(user => user.UserEmail == email);
            return user;
        }
        public void UpdateUserRefreshToken(Users user ,int refreshTokenId)
        {
            if (refreshTokenId == 0)
            {
                user.RefreshTokenId = null;
            }
            else
            {
                user.RefreshTokenId = refreshTokenId;
            }
        }
        public IEnumerable<Users> GetUsers(string? searchQuery, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                var users = GetAll().OrderBy(u => u.UserName).Skip(pageSize * (pageNumber - 1)).Take(pageSize); 
                return users;
            } 
            else
            {
                var users = Find(u=>u.UserName.Contains(searchQuery)).OrderBy(u => u.UserName).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                return users;
            }
        }
    }
}
