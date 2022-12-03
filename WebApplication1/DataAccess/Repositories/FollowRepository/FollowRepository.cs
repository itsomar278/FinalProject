using System;
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
        public void FollowUser(int userId, int userToFollowId)
        {
            Follow follow = new Follow
            {
                FollowedId = userToFollowId,
                FollowerId = userId
            };
            Add(follow);
        }
        public Follow Get(int followerId, int followedId)
        {
           var follow = _DbContext.Set<Follow>().Where(f => f.FollowerId== followerId && f.FollowedId == followedId).FirstOrDefault();
            return follow;
        }

        public List<int> GetAllFollowersId(int userId)
        {
            List<int> followersId = new List<int>();
             var followers =_DbContext.Set<Follow>().Where(f =>f.FollowedId == userId);
            foreach (var f in followers)
            {
                followersId.Add(f.FollowerId);
            }
            return followersId;
        }
        public List<int> GetAllFollowingId(int userId)
        {
            List<int> followingId = new List<int>();
            var followers = _DbContext.Set<Follow>().Where(f => f.FollowerId == userId);
            foreach (var f in followers)
            {
                followingId.Add(f.FollowedId);
            }
            return followingId;
        }
        override
            public Follow Get(int id)
        {
            throw new ArgumentException("unsupported opearation");
        }
    }
}
