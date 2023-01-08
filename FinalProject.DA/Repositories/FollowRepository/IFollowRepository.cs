using DataAcess.Entites;
using DataAcess.Repositories;
using System.Xml.Serialization;

namespace DataAcess.Repositories.FollowRepository
{
    public interface IFollowRepository : IRepository<Follow>
    {
        public Task<List<int>> GetAllFollowersIdAsync(int userId);
        public Task<List<int>> GetAllFollowingId(int userId);
        public void FollowUser(int userId, int userToFollowId);
    }
}
