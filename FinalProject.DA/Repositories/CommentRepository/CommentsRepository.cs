using DataAcess;
using DataAcess.Entites;

namespace DataAcess.Repositories.CommentRepository
{
    public class CommentsRepository : Repository<Comments>, ICommentsRepository
    {
        public CommentsRepository(ProjectDbContext projectDbContext) : base(projectDbContext)
        {
        }

    }
}
