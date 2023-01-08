using DataAcess.Entites;
using DataAcess.Repositories;
using System.Linq.Expressions;
using System.Security.Claims;

namespace DataAcess.Repositories.UsersRepository
{
    public interface IUsersRepository : IRepository<Users>
    {
        public Task<Users> FindByEmailAsync(string email);
        public void UpdateUserRefreshToken(int userId, int refreshTokenId);
        public Task<IEnumerable<Users>> GetUsersAsync(string? searchQuery, int pageNumber, int pageSize);
    }
}
