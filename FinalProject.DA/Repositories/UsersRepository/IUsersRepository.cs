using System.Linq.Expressions;
using System.Security.Claims;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.UsersRepository
{
    public interface IUsersRepository : IRepository<Users>
    {
        public Users FindByEmail(string email);
        public void UpdateUserRefreshToken(int userId, int refreshTokenId);
        public IEnumerable<Users> GetUsers(string? searchQuery, int pageNumber, int pageSize);
    }
}
