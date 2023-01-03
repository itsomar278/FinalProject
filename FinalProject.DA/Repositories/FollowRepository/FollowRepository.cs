using DataAcess.Entites;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAcess.Repositories.FollowRepository
{
    public class FollowRepository : Repository<Follow>, IFollowRepository
    {
        public FollowRepository(ProjectDbContext projectDbContext) : base(projectDbContext)
        {
        }

        public void FollowUser(int userId, int userToFollowId)
        {
            Follow follow = new Follow
            {
                FollowedId = userToFollowId,
                FollowerId = userId
            };
            AddAsync(follow);
        }
        public async Task<List<int>> GetAllFollowersIdAsync(int userId)
        {
            return await _DbContext.Set<Follow>()
                    .Where(f => f.FollowedId == userId)
                    .Select(f => f.FollowerId)
                    .ToListAsync();
        }
        public async Task<List<int>> GetAllFollowingId(int userId)
        {
            return await _DbContext.Set<Follow>()
                   .Where(f => f.FollowerId == userId)
                   .Select(f => f.FollowedId)
                   .ToListAsync();
        }
    }
}
