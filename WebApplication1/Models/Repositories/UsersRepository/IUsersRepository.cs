using System.Linq.Expressions;
using WebApplication1.Models.Entites;

namespace WebApplication1.Models.Repositories.UsersRepository
{
    public interface IUsersRepository : IRepository<Users>
    {
        public Users FindByEmail(string email);
    }
}
