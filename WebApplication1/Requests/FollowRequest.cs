using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Requests
{
    public class FollowRequest
    {
        [Required]
        public int UserToFollowId { get; set; }
    }
}
