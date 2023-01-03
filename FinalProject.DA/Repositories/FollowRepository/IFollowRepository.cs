﻿using System.Xml.Serialization;
using WebApplication1.Models.Entites;

namespace WebApplication1.DataAccess.Repositories.FollowRepository
{
    public interface IFollowRepository : IRepository<Follow>
    {
        public Task<List<int>> GetAllFollowersIdAsync(int userId);
        public Task<List<int>> GetAllFollowingId(int userId);
        public void FollowUser(int userId , int userToFollowId);
    }
}
