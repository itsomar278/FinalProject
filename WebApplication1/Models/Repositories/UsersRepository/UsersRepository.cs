using System.Linq;
using System.Linq.Expressions;
using WebApplication1.Models.DataAccess;
using WebApplication1.Models.Entites;

namespace WebApplication1.Models.Repositories.UsersRepository
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
    }
}
