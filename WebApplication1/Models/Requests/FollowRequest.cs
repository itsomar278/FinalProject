using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Requests
{
    public class FollowRequest
    {
        [Required]
        public int UserToFollowId { get; set; }
    }
}
