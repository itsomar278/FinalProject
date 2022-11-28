using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.FollowRepository
{
    public class FollowRepository : Repository<Follow>, IFollowRepository
    {
        public FollowRepository(ProjectDbContext projectDbContext) : base(projectDbContext)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
    }
}
