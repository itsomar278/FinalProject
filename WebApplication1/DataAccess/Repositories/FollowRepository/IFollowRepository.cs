using System.Xml.Serialization;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.FollowRepository
{
    public interface IFollowRepository : IRepository<Follow>
    {
        public List<int> GetAllFollowersId(int userId);
        public List<int> GetAllFollowingId(int userId);
        public void FollowUser(int userId , int userToFollowId);
        public Follow Get(int followerId , int followedId);
    }
}
