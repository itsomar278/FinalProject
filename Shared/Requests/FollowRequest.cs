using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests
{
    public class FollowRequest
    {
        [Required]
        public int UserToFollowId { get; set; }
    }
}
