using WebApplication1.Models.DataAccess;
using WebApplication1.Models.Entites;

namespace WebApplication1.Models.Repositories.CommentsRepository
{
    public class CommentsRepository : Repository<Comments> , ICommentsRepository
    {
        public CommentsRepository(ProjectDbContext projectDbContext) : base(projectDbContext)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
    }
}
