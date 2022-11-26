using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Follow
    {
        [Required]
        public int FollowerId { get; set; }
        public Users FollowerUser { get; set; }
        [Required]
        public int FollowedId { get; set; }
        public Users FollowedUser { get; set; }
    }
}
