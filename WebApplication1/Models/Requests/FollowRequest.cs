using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class FollowRequest
    {
        [Required]
        public int FollowerId { get; set; }
        [Required]
        public int FollowedId { get; set; }
    }
}
